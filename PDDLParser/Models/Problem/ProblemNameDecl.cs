using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class ProblemNameDecl : BaseNode, IDecl
    {
        public string Name { get; set; }

        public ProblemNameDecl(ASTNode node, string name) : base(node)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(problem {Name})";
        }

        public override List<INode> FindName(string name)
        {
            if (Name == name)
                return new List<INode>() { this };
            return new List<INode>();
        }
    }
}
