using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class NotExp : BaseNode, IExp
    {
        public IExp Child { get; set; }

        public NotExp(ASTNode node, INode parent, IExp child) : base(node, parent)
        {
            Child = child;
        }

        public override string ToString()
        {
            return $"(not {Child})";
        }

        public override HashSet<INode> FindName(string name)
        {
            return Child.FindName(name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Child.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NotExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
