using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypesDecl : IDecl
    {
        public List<TypeDecl> Types { get; set; }

        public TypesDecl(List<TypeDecl> types)
        {
            Types = types;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach(var type in Types)
                retStr += $"{type}{Environment.NewLine}";
            return $"(:types{retStr})";
        }
    }
}
