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
        public TypeNameDecl TypeName { get; set; }
        public List<TypeNameDecl> SubTypes { get; set; }

        public TypeDecl(ASTNode node, TypeNameDecl name, List<TypeNameDecl> subTypes) : base(node)
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

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            res.AddRange(TypeName.FindName(name));
            foreach (var subtype in SubTypes)
                res.AddRange(subtype.FindName(name));
            return res;
        }
    }
}
