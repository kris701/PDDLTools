using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class Predicate : IExp
    {
        public string Name { get; set; }
        public List<NameNode> Arguments { get; set; }
    }
}
