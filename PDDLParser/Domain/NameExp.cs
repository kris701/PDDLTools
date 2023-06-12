using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class NameExp : IExp
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public NameExp(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public NameExp(string name)
        {
            Name = name;
            Type = "";
        }

        public override string ToString()
        {
            if (Type == "")
                return $"({Name})";
            else
                return $"({Name} - {Type})";
        }
    }
}
