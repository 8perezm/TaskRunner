using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRunner.Task;

namespace TaskRunner.Trigger;

public interface ITriggerCommand
{
    void Execute(ITaskRunner parent, Model.Trigger trigger);
}
