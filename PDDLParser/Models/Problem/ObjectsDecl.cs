using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class ObjectsDecl : BaseNode, IDecl
    {
        public List<NameExp> Objs { get; set; }

        public ObjectsDecl(ASTNode node, INode parent, List<NameExp> types) : base(node, parent)
        {
            Objs = types;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Objs)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:objects{retStr})";
        }

        public override HashSet<INode> FindNames(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var obj in Objs)
                res.AddRange(obj.FindNames(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var obj in Objs)
                hash *= obj.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ObjectsDecl exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
