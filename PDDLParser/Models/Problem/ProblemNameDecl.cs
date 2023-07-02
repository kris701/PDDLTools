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

        public ProblemNameDecl(ASTNode node, INode parent, string name) : base(node, parent)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(problem {Name})";
        }

        public override HashSet<INode> FindNames(string name)
        {
            if (Name == name)
                return new HashSet<INode>() { this };
            return new HashSet<INode>();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is ProblemNameDecl exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
