using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class RequirementsDecl : IDecl
    {
        public List<string> Requirements {  get; set; }

        public RequirementsDecl(List<string> requirements)
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
