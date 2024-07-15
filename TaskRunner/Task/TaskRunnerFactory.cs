using TaskRunner.Model;
using TaskRunner.TaskCommand;
using TaskRunner.Trigger;

namespace TaskRunner.Task;

public class TaskRunnerFactory : ITaskRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TaskRunnerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITaskRunner CreateTaskRunner(TaskDefinition taskDefinition)
    {
        var logger = _serviceProvider.GetRequiredService<ILogger<TaskRunner>>();
        var triggerCommandFactory = _serviceProvider.GetRequiredService<ITriggerCommandFactory>();
        var taskCommandFactory = _serviceProvider.GetRequiredService<ITaskCommandFactory>();

        return new TaskRunner(logger, triggerCommandFactory, taskCommandFactory, _serviceProvider, taskDefinition);
    }
}