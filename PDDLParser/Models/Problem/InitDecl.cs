using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class InitDecl : IDecl
    {
        public List<NameExp> Predicates { get; set; }

        public InitDecl(List<NameExp> predicates)
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
    }
}
