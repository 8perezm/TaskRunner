using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRunner.Task;

namespace TaskRunner.Trigger;

public class ScheduleTriggerCommand : ITriggerCommand
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private readonly ILogger<ScheduleTriggerCommand> _logger;

    public ScheduleTriggerCommand(ILogger<ScheduleTriggerCommand> logger)
    {
        _logger = logger;
    }
    public void Execute(ITaskRunner taskRunner, Model.Trigger trigger)
    {
        if(trigger.Input.TryGetValue("cron", out var cronExpression))
        {
            var schedule = CrontabSchedule.Parse(cronExpression);
            var nextRun = schedule.GetNextOccurrence(DateTime.Now);

            System.Threading.Tasks.Task.Run(async () =>
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var now = DateTime.Now;
                    var nextrun = schedule.GetNextOccurrence(now);
                    if (now >= nextRun)
                    {
                        try
                        {
                            taskRunner.RunTasks();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error executing scheduled task: {ex.Message}");
                        }
                        nextRun = schedule.GetNextOccurrence(DateTime.Now);
                    }
                    await System.Threading.Tasks.Task.Delay(1000); // Check every 10 seconds
                }
            }, _cancellationTokenSource.Token);
        }
        else
        {
            _logger.LogError("Cron expression not found");
        }
    }
}
