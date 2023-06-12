using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class ProblemNameDecl : IDecl
    {
        public string Name { get; set; }

        public ProblemNameDecl(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(problem {Name})";
        }
    }
}
