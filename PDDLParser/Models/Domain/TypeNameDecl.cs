using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypeNameDecl : BaseNode, IExp, INamedNode
    {
        public string Name { get; set; }

        public TypeNameDecl(ASTNode node, INode parent, string name) : base(node, parent)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeNameDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            if (Name == name)
                return new HashSet<INamedNode>() { this };
            return new HashSet<INamedNode>();
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            return res;
        }

    }
}
