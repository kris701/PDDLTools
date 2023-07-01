using PDDLParser.AST;
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

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            foreach (var predicate in Predicates)
                res.AddRange(predicate.FindName(name));
            return res;
        }
    }
}
