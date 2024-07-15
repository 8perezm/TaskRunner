using System;
using System.Collections.Generic;

namespace TaskRunner.TaskCommand;

public class TaskCommandFactory : ITaskCommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TaskCommandTypeDictionary _taskCommandTypes;

    public TaskCommandFactory(IServiceProvider serviceProvider, TaskCommandTypeDictionary taskCommandTypes)
    {
        _serviceProvider = serviceProvider;
        _taskCommandTypes = taskCommandTypes;
    }

    public ITaskCommand? CreateTaskCommand(string taskType)
    {
        if (_taskCommandTypes.TryGetValue(taskType.ToLower(), out var type))
        {
            return _serviceProvider.GetRequiredService(type) as ITaskCommand;
        }
        throw new ArgumentException($"Unknown task type: {taskType}");
    }
}