using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class DomainNameDecl : BaseNode, IDecl
    {
        public string Name { get; set; }

        public DomainNameDecl(ASTNode node, string name) : base(node)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(domain {Name})";
        }

        public override HashSet<INode> FindName(string name)
        {
            if (Name == name)
                return new HashSet<INode>() { this };
            return new HashSet<INode>();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is DomainNameDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
