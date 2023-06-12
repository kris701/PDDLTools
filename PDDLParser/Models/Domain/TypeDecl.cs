using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypeDecl : BaseNode, IDecl
    {
        public string TypeName { get; set; }
        public List<string> SubTypes { get; set; }

        public TypeDecl(ASTNode node, string name, List<string> subTypes) : base(node)
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
