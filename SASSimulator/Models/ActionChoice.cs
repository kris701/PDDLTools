using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator.Models
{
    public class ActionChoice
    {
        public string Name { get; set; }
        public List<string> Arguments { get; set; }

        public ActionChoice(string name, List<string> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public override string ToString()
        {
            var argStr = "";
            foreach (var arg in Arguments)
                argStr += $" {arg}";
            return $"({Name}{argStr})";
        }
    }
}
