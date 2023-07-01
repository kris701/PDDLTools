using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypeNameDecl : BaseNode, IExp
    {
        public string Name { get; set; }

        public TypeNameDecl(ASTNode node, string name) : base(node)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeNameDecl exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public override List<INode> FindName(string name)
        {
            if (Name == name)
                return new List<INode>() { this };
            return new List<INode>();
        }
    }
}
