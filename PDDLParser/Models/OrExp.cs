using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
