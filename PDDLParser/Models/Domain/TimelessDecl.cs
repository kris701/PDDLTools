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
        public List<NameExp> Objs { get; set; }

        public TimelessDecl(ASTNode node, List<NameExp> types) : base(node)
        {
            Objs = types;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Objs)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:timeless{retStr})";
        }
    }
}
