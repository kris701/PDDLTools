using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class ConstantsDecl : IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(List<NameExp> constants)
        {
            Constants = constants;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Constants)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:constants{retStr})";
        }
    }
}
