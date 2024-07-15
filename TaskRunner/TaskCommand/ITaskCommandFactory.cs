using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.TaskCommand;

public interface ITaskCommandFactory
{
    ITaskCommand? CreateTaskCommand(string taskType);
}
