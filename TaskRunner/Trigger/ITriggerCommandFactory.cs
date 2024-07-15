namespace TaskRunner.Trigger;

public interface ITriggerCommandFactory
{
    ITriggerCommand CreateTriggerCommand(string triggerName);
}
