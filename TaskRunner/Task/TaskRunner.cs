using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRunner.Model;
using TaskRunner.TaskCommand;
using TaskRunner.Trigger;

namespace TaskRunner.Task;

public class TaskRunner : ITaskRunner
{
    private readonly ITaskCommandFactory _taskCommandFactory;
    private readonly ILogger<TaskRunner> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly TaskDefinition _taskDefinition;
    private readonly ITriggerCommandFactory _triggerCommandFactory;

    public TaskRunner(ILogger<TaskRunner> logger, ITriggerCommandFactory triggerCommandFactory, ITaskCommandFactory taskCommandFactory, IServiceProvider serviceProvider, TaskDefinition taskDefinition)
    {
        _taskCommandFactory = taskCommandFactory;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _taskDefinition = taskDefinition;
        _triggerCommandFactory = triggerCommandFactory;

        //_logger.LogInformation($"TaskRunner created with {tasks.Count} tasks");
    }

    public void RunTrigger()
    {
        try
        {
            var triggerCommand = _triggerCommandFactory.CreateTriggerCommand(_taskDefinition.Trigger.Name);
            triggerCommand.Execute(this, _taskDefinition.Trigger);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running trigger");
        }
    }

    public void RunTasks()
    {
        var tasks = _taskDefinition.Tasks;
        foreach (var task in tasks)
        {
            _logger.LogInformation($"Executing task: {task.DisplayName}");
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var taskCommand = _taskCommandFactory.CreateTaskCommand(task.Task);
                    taskCommand.Execute(task);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error executing task: {task.DisplayName}");
                }
            }
        }
    }
}
