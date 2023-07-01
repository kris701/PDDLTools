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

        public bool ContainsType(TypeNameDecl typeName) => ContainsType(typeName.Name);
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
        public bool IsTypeOrSubType(TypeNameDecl typeName, TypeNameDecl targetType) => IsTypeOrSubType(typeName.Name, targetType.Name);
        public bool IsTypeOrSubType(string typeName, string targetType)
        {
            if (typeName == targetType)
                return true;

            if (TypesTable.ContainsKey(targetType))
            {
                if (TypesTable[targetType].Contains(typeName))
                    return true;
            }

            return false;
        }
        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();

            res.AddRange(Name.FindName(name));
            res.AddRange(Requirements.FindName(name));
            res.AddRange(Extends.FindName(name));
            res.AddRange(Timeless.FindName(name));
            res.AddRange(Types.FindName(name));
            res.AddRange(Constants.FindName(name));
            res.AddRange(Predicates.FindName(name));
            foreach(var act in Actions)
                res.AddRange(act.FindName(name));
            foreach(var axi in Axioms)
                res.AddRange(axi.FindName(name));

            return res;
        }

        public DomainDecl(ASTNode node) : base(node) {
            Actions = new List<ActionDecl>();
            Axioms = new List<AxiomDecl>();
        }
    }
}
