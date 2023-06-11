using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class ActionStmt
    {
        public string Name { get; set; }

        public List<NameExp> Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }
    }
}
