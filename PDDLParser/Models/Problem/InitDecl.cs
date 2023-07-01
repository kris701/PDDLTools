using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class InitDecl : BaseNode, IDecl
    {
        public List<PredicateExp> Predicates { get; set; }

        public InitDecl(ASTNode node, List<PredicateExp> predicates) : base(node)
        {
            Predicates = predicates;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Predicates)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:init{retStr})";
        }

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var predicate in Predicates)
                res.AddRange(predicate.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var pred in Predicates)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is InitDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
