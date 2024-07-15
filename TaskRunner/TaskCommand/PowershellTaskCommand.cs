using System;
using System.Diagnostics;
using TaskRunner.Model;

namespace TaskRunner.TaskCommand;

public class PowershellTaskCommand : ITaskCommand
{
    private readonly ILogger<PowershellTaskCommand> _logger;

    public PowershellTaskCommand(ILogger<PowershellTaskCommand> logger)
    {
        _logger = logger;
    }

    public void Execute(TaskItem task)
    {
        if (task.Inputs.TryGetValue("command", out var powershellCommand))
        {
            var processInfo = new ProcessStartInfo("powershell", $"-Command \"{powershellCommand}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(processInfo);
            process?.WaitForExit();
            var output = process?.StandardOutput.ReadToEnd();
            var error = process?.StandardError.ReadToEnd();

            _logger.LogInformation($"PowerShell Command Output: {output}");
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogError($"PowerShell Command Error: {error}");
            }
        }
    }
}