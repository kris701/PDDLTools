using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class RequirementsDecl : BaseNode, IDecl
    {
        public List<string> Requirements {  get; set; }

        public RequirementsDecl(ASTNode node, List<string> requirements) : base(node)
        {
            Requirements = requirements;
        }

        public override string ToString()
        {
            var reqStr = "";
            foreach (var requirement in Requirements)
                reqStr += $" {requirement}";
            return $"(:requirements{reqStr})";
        }
    }
}
