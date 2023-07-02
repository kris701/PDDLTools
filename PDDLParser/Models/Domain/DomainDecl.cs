using PDDLParser.AST;
using PDDLParser.Exceptions;
using PDDLParser.Helpers;
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
        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();

            if (Name != null)
                res.AddRange(Name.FindName(name));
            if (Requirements != null)
                res.AddRange(Requirements.FindName(name));
            if (Extends != null)
                res.AddRange(Extends.FindName(name));
            if (Timeless != null)
                res.AddRange(Timeless.FindName(name));
            if (Types != null)
                res.AddRange(Types.FindName(name));
            if (Constants != null)
                res.AddRange(Constants.FindName(name));
            if (Predicates != null)
                res.AddRange(Predicates.FindName(name));
            if (Actions != null)
                foreach(var act in Actions)
                    res.AddRange(act.FindName(name));
            if (Axioms != null)
                foreach(var axi in Axioms)
                    res.AddRange(axi.FindName(name));

            return res;
        }

        public DomainDecl(ASTNode node) : base(node, null) {
            Actions = new List<ActionDecl>();
            Axioms = new List<AxiomDecl>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();

            if (Name != null)
                hash *= Name.GetHashCode();
            if (Requirements != null)
                hash *= Requirements.GetHashCode();
            if (Extends != null)
                hash *= Extends.GetHashCode();
            if (Timeless != null)
                hash *= Timeless.GetHashCode();
            if (Types != null)
                hash *= Types.GetHashCode();
            if (Constants != null)
                hash *= Constants.GetHashCode();
            if (Predicates != null)
                hash *= Predicates.GetHashCode();
            if (Actions != null)
                foreach(var act in Actions)
                    hash *= act.GetHashCode();
            if (Axioms != null)
                foreach(var axi in Axioms)
                    hash *= axi.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is DomainDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
