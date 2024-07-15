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
    private Dictionary<string, ITaskRunner> taskRunners = new Dictionary<string, ITaskRunner>();

    public TaskRunnerService(ILogger<TaskRunnerService> logger, string folderPath, ITaskRunnerFactory taskRunnerFactory)
    {
        _folderPath = folderPath;
        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        _taskRunnerFactory = taskRunnerFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ScanFolderAndRunTasks();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Adjust the delay based on your trigger cron settings
        }
    }

    private void ScanFolderAndRunTasks()
    {
        var yamlFiles = Directory.GetFiles(_folderPath, "*.yaml");
        if(yamlFiles == null || yamlFiles.Length == 0)
        {
            _logger.LogInformation("No tasks found in the folder");
            return;
        }
        else
        {
            foreach (var file in yamlFiles)
            {
                var yamlContent = File.ReadAllText(file);
                var taskDefinition = _deserializer.Deserialize<TaskDefinition>(yamlContent);

                var yamlContentWithVars = ReplaceVariables(taskDefinition.Variables, yamlContent);
                taskDefinition = _deserializer.Deserialize<TaskDefinition>(yamlContentWithVars);

                var taskRunner = _taskRunnerFactory.CreateTaskRunner(taskDefinition);
                taskRunner.RunTrigger();

                taskRunners.Add(file, taskRunner);
            }
        }
    }

    private string ReplaceVariables(Dictionary<string, string> vars, string yamlContent)
    {
        string pattern = @"\$\((.*?)\)";

        // Replacing the placeholders with values from the dictionary
        string result = Regex.Replace(yamlContent, pattern, match =>
        {
            string key = match.Groups[1].Value;
            return vars.TryGetValue(key, out string value) ? value : match.Value;
        });

        return result;
    }
}
