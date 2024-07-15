using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Model;

public class TaskItem
{
    public string Task { get; set; }
    public string DisplayName { get; set; }
    public Dictionary<string, string> Inputs { get; set; }
}
