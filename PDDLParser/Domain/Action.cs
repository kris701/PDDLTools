using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class Action
    {
        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }
    }
}
