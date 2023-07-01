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

        public DomainNameRefDecl(ASTNode node, string name) : base(node)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(:domain {Name})";
        }

        public override List<INode> FindName(string name)
        {
            if (Name == name)
                return new List<INode>() { this };
            return new List<INode>();
        }
    }
}
