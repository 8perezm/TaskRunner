using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Model;

public class TaskDefinition
{
    public Trigger Trigger { get; set; }
    public List<TaskItem> Tasks { get; set; }
    public Dictionary<string, string> Variables { get; set; }
}
