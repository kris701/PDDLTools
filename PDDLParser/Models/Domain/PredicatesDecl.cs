using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class PredicatesDecl : IDecl
    {
        public List<PredicateDecl> Predicates { get; set; }

        public PredicatesDecl(List<PredicateDecl> predicates)
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
