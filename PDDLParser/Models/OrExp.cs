using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Models
{
    public class OrExp : BaseNode, IExp
    {
        public IExp Option1 { get; set; }
        public IExp Option2 { get; set; }

        public OrExp(ASTNode node, IExp option1, IExp option2) : base(node)
        {
            Option1 = option1;
            Option2 = option2;
        }

        public override string ToString()
        {
            return $"(or {Option1} {Option2})";
        }

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            res.AddRange(Option1.FindName(name));
            res.AddRange(Option2.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Option1.GetHashCode() * Option2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is OrExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
