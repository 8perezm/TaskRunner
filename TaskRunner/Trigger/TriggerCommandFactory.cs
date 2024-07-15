using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRunner.TaskCommand;

namespace TaskRunner.Trigger;

public class TriggerCommandFactory : ITriggerCommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TriggerCommandTypeDictionary _triggerCommandTypes;
    public TriggerCommandFactory(IServiceProvider serviceProvider, TriggerCommandTypeDictionary triggerCommandTypes)
    {
        _serviceProvider = serviceProvider;
        _triggerCommandTypes = triggerCommandTypes;
    }
    public ITriggerCommand? CreateTriggerCommand(string triggerName)
    {
        if (_triggerCommandTypes.TryGetValue(triggerName.ToLower(), out var type))
        {
            return _serviceProvider.GetRequiredService(type) as ITriggerCommand;
        }
        throw new ArgumentException($"Unknown task type: {triggerName}");
    }
}
