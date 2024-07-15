using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskRunner.Model;

public class Trigger
{
    public string Name { get; set; }
    public Dictionary<string, string> Input { get; set; }
}