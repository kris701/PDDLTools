using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class PredicatesDecl : BaseNode, IDecl
    {
        public List<PredicateDecl> Predicates { get; set; }

        public PredicatesDecl(ASTNode node, List<PredicateDecl> predicates) : base(node)
        {
            Predicates = predicates;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Predicates)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:predicates{retStr})";
        }
    }
}
