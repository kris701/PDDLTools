using PDDLParser.AST;
using PDDLParser.Helpers;
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

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var item in Items)
                res.AddRange(item.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var item in Items)
                hash *= item.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimelessDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
