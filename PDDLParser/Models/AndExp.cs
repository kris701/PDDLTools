using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class AndExp : BaseNode, IExp
    {
        public List<IExp> Children { get; set; }

        public AndExp(ASTNode node, INode parent, List<IExp> children) : base(node, parent)
        {
            Children = children;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Children)
                retStr += $" {type}{Environment.NewLine}";
            return $"(and{retStr})";
        }

        public override HashSet<INode> FindNames(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var child in Children)
                res.AddRange(child.FindNames(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var child in Children)
                hash *= child.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is AndExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
