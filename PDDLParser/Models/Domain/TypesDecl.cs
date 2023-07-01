using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypesDecl : BaseNode, IDecl
    {
        public List<TypeDecl> Types { get; set; }

        public TypesDecl(ASTNode node, List<TypeDecl> types) : base(node)
        {
            Types = types;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach(var type in Types)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:types{retStr})";
        }

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            foreach (var type in Types)
                res.AddRange(type.FindName(name));
            return res;
        }
    }
}
