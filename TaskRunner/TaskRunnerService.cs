using System.Text.RegularExpressions;
using TaskRunner.Model;
using TaskRunner.Task;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class TaskRunnerService : BackgroundService
{
    private readonly string _folderPath;
    private readonly IDeserializer _deserializer;
    private readonly ITaskRunnerFactory _taskRunnerFactory;
    private readonly ILogger _logger;
    private Dictionary<string, (ITaskRunner taskRunner, DateTime lastModified, CancellationTokenSource cts)> taskRunners = new Dictionary<string, (ITaskRunner, DateTime, CancellationTokenSource)>();

    public TaskRunnerService(ILogger<TaskRunnerService> logger, string folderPath, ITaskRunnerFactory taskRunnerFactory)
    {
        _folderPath = folderPath;
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _taskRunnerFactory = taskRunnerFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            ScanFolderAndRunTasks();
            await Task.Delay(TimeSpan.FromMinutes(1), ct); // Adjust the delay based on your trigger cron settings
        }
    }

    private void ScanFolderAndRunTasks()
    {
        var yamlFiles = Directory.GetFiles(_folderPath, "*.yaml");
        if (yamlFiles == null || yamlFiles.Length == 0)
        {
            _logger.LogInformation("No tasks found in the folder");
            return;
        }
        else
        {
            foreach (var file in yamlFiles)
            {
                var lastModified = File.GetLastWriteTime(file);

                if (!taskRunners.ContainsKey(file) || taskRunners[file].lastModified < lastModified)
                {
                    // Read the yaml content and deserialize it
                    var yamlContent = File.ReadAllText(file);
                    var taskDefinition = _deserializer.Deserialize<TaskDefinition>(yamlContent);

                    // Replace variables in the yaml content
                    var yamlContentWithVars = ReplaceVariables(taskDefinition.Variables, yamlContent);
                    taskDefinition = _deserializer.Deserialize<TaskDefinition>(yamlContentWithVars);

                    var cts = new CancellationTokenSource();

                    // Cancel the old task runner if the file has been modified
                    if(taskRunners.ContainsKey(file))
                    {
                        _logger.LogInformation($"File {file} has been modified. Removing old task runner");
                        taskRunners[file].cts.Cancel();
                    }

                    _logger.LogInformation($"Creating task runner for file {file}");
                    var taskRunner = _taskRunnerFactory.CreateTaskRunner(taskDefinition, cts.Token);
                    taskRunners[file] = (taskRunner, lastModified, cts);
                    taskRunner.RunTrigger();
                }
            }

            // Remove task runners for files that have been deleted
            var filesSet = yamlFiles.ToHashSet();
            var keysToRemove = taskRunners.Keys.Where(k => !filesSet.Contains(k)).ToList();
            foreach (var key in keysToRemove)
            {
                _logger.LogInformation($"Removing task runner for file {key}");
                taskRunners[key].cts.Cancel();
                taskRunners.Remove(key);
            }
        }
    }

    private string ReplaceVariables(Dictionary<string, string> vars, string yamlContent)
    {
        // Regex pattern to match $(**)
        string pattern = @"\$\((.*?)\)";

        // Replacing the placeholders with values from the dictionary
        string result = Regex.Replace(yamlContent, pattern, match =>
        {
            string key = match.Groups[1].Value;
            return vars.TryGetValue(key, out var value) ? value : match.Value;
        });

        return result;
    }
}
