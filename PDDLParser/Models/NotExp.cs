using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class NotExp : IExp
    {
        public IExp Child { get; set; }

        public NotExp(IExp child)
        {
            Child = child;
        }

        public override string ToString()
        {
            return $"(not {Child})";
        }
    }
}
