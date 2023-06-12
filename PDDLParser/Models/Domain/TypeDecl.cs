using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypeDecl : IDecl
    {
        public string TypeName { get; set; }
        public List<string> SubTypes { get; set; }

        public TypeDecl(string name, List<string> subTypes)
        {
            TypeName = name;
            SubTypes = subTypes;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var subType in SubTypes)
                retStr += $"{subType} ";
            return $"{retStr} - {TypeName}";
        }
    }
}
