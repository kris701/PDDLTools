using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.ErrorList.PDDLParser.Domain
{
    public class Predicate
    {
        public string Name { get; set; }
        public List<string> Arguments { get; set; }
    }
}
