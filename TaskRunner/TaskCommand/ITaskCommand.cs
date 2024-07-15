using TaskRunner.Model;

namespace TaskRunner.TaskCommand;

public interface ITaskCommand
{
    void Execute(TaskItem task);
}
