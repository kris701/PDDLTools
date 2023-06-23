using PDDLParser.AST;
using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class DomainDecl : BaseNode, IDecl
    {
        public DomainNameDecl Name { get; set; }
        public RequirementsDecl Requirements { get; set; }
        public ExtendsDecl Extends { get; set; }
        public TimelessDecl Timeless { get; set; }
        public TypesDecl Types { get; set; }
        public ConstantsDecl Constants { get; set; }
        public PredicatesDecl Predicates { get; set; }
        public List<ActionDecl> Actions { get; set; }
        public List<AxiomDecl> Axioms { get; set; }

        // Context
        public Dictionary<string, List<string>> PredicateTypeTable { get; internal set; }
        public Dictionary<string, List<string>> TypesTable { get; internal set; }

        public bool ContainsType(string typeName)
        {
            if (TypesTable == null)
                return false;
            if (TypesTable.ContainsKey(typeName))
                return true;
            foreach (var subTypes in TypesTable.Values)
                if (subTypes.Any(x => x == typeName))
                    return true;
            return false;
        }

        public DomainDecl(ASTNode node) : base(node) {
            Actions = new List<ActionDecl>();
            Axioms = new List<AxiomDecl>();
        }
    }
}
