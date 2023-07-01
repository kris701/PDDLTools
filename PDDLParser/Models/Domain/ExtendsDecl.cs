using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Models.Domain
{
    public class ExtendsDecl : BaseNode, IDecl
    {
        public List<NameExp> Extends { get; set; }

        public ExtendsDecl(ASTNode node, List<NameExp> extends) : base(node)
        {
            Extends = extends;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Extends)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:extends{retStr})";
        }

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var extend in Extends)
                res.AddRange(extend.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var extend in Extends)
                hash *= extend.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ExtendsDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
