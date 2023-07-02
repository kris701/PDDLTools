using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Models.Domain
{
    public class ConstantsDecl : BaseNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, INode parent, List<NameExp> constants) : base(node, parent) 
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

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var cons in Constants)
                res.AddRange(cons.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var constant in Constants)
                hash *= constant.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ConstantsDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
