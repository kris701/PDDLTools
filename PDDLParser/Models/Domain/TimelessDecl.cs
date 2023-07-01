using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TimelessDecl : BaseNode, IDecl
    {
        public List<PredicateExp> Items { get; set; }

        public TimelessDecl(ASTNode node, List<PredicateExp> timeless) : base(node)
        {
            Items = timeless;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Items)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:timeless{retStr})";
        }

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            foreach (var item in Items)
                res.AddRange(item.FindName(name));
            return res;
        }
    }
}
