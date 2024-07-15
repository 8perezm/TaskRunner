using System.Diagnostics;
using TaskRunner.Model;

namespace TaskRunner.TaskCommand;

public class BashTaskCommand : ITaskCommand
{
    private readonly ILogger<BashTaskCommand> _logger;

    public BashTaskCommand(ILogger<BashTaskCommand> logger)
    {
        _logger = logger;
    }

    public void Execute(TaskItem task)
    {
        if (task.Inputs.TryGetValue("command", out var bashCommand))
        {
            var processInfo = new ProcessStartInfo("/bin/bash", $"-c \"{bashCommand}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
            Console.WriteLine(process.StandardOutput.ReadToEnd());
            Console.Error.WriteLine(process.StandardError.ReadToEnd());
        }
    }
}