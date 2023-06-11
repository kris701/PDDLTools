using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class TypeDecl
    {
        public string Name { get; set; }
        public List<string> SubTypes { get; set; }

        public TypeDecl(string name, List<string> subTypes)
        {
            Name = name;
            SubTypes = subTypes;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach(var subType in SubTypes)
                retStr += $"{subType} ";
            return $"{retStr} - {Name}";
        }
    }
}
