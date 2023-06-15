using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class ExtendsDecl : BaseNode, IDecl
    {
        public List<string> Extends { get; set; }

        public ExtendsDecl(ASTNode node, List<string> extends) : base(node)
        {
            Extends = extends;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Extends)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:extends{retStr})";
        }
    }
}
