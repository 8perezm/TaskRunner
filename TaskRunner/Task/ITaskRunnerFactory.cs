using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskRunner.Model;

namespace TaskRunner.Task;

public interface ITaskRunnerFactory
{
    ITaskRunner CreateTaskRunner(TaskDefinition taskDefinition, CancellationToken ct);
}
