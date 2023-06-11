using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.ErrorList.PDDLParser.Domain
{
    public class DomainFile
    {
        public string Name { get; set; }
        public List<Predicate> Predicates { get; set; }
        public List<Action> Actions { get; set; }

        public DomainFile(string parseFile)
        {

        }
    }
}
