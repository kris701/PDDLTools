using PDDLParser.AST;
using PDDLParser.Helpers;
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

        public TypesDecl(ASTNode node, INode parent, List<TypeDecl> types) : base(node, parent)
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

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var type in Types)
                res.AddRange(type.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var type in Types)
                hash *= type.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypesDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
