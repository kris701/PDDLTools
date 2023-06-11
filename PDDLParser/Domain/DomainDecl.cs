using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class DomainDecl
    {
        public string Name { get; set; }
        public List<string> Requirements { get; set; }
        public List<TypeDecl> Types { get; set; }
        public List<NameExp> Constants { get; set; }
        public List<PredicateStmt> Predicates { get; set; }
        public List<ActionStmt> Actions { get; set; }

        public DomainDecl(string name, List<string> requirements, List<TypeDecl> types, List<NameExp> constants, List<PredicateStmt> predicates, List<ActionStmt> actions)
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
