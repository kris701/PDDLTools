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

        public NotExp(ASTNode node, IExp child) : base(node)
        {
            Child = child;
        }

        public override string ToString()
        {
            return $"(not {Child})";
        }
    }
}
