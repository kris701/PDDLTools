using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class AndExp : IExp
    {
        public List<IExp> Children { get; set; }

        public AndExp(List<IExp> children)
        {
            Children = children;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Children)
                retStr += $" {type}{Environment.NewLine}";
            return $"(and{retStr})";
        }
    }
}
