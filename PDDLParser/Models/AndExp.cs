using PDDLParser.AST;
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

        public AndExp(ASTNode node, List<IExp> children) : base(node)
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

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            foreach (var child in Children)
                res.AddRange(child.FindName(name));
            return res;
        }
    }
}
