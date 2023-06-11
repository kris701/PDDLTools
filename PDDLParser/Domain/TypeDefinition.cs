using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class TypeDefinition
    {
        public string Name { get; set; }
        public List<string> SubTypes { get; set; }
    }
}
