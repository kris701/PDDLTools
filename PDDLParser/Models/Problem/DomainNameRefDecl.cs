using PDDLParser.AST;
using PDDLParser.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class DomainNameRefDecl : BaseNode, IDecl
    {
        public string Name { get; set; }

        // Context
        public DomainDecl DomainReference { get; internal set; }

        public DomainNameRefDecl(ASTNode node, INode parent, string name) : base(node, parent)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(:domain {Name})";
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
            if (obj is DomainNameRefDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
