using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class DomainFile
    {
        public string Name { get; set; }
        public List<string> Requirements { get; set; }
        public List<TypeDefinition> Types { get; set; }
        public List<NameNode> Constants { get; set; }
        public List<Predicate> Predicates { get; set; }
        public List<Action> Actions { get; set; }

        public DomainFile(string name, List<string> requirements, List<TypeDefinition> types, List<NameNode> constants, List<Predicate> predicates, List<Action> actions)
        {
            Name = name;
            Requirements = requirements;
            Types = types;
            Constants = constants;
            Predicates = predicates;
            Actions = actions;
        }
    }
}
