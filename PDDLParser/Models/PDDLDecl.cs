using PDDLParser.Helpers;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class PDDLDecl
    {
        public DomainDecl Domain { get; set; }
        public ProblemDecl Problem { get; set; }

        public PDDLDecl(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
        }

        public HashSet<INode> FindNames(string name)
        {
            var matches = new HashSet<INode>();
            if (Domain != null)
                matches.AddRange(Domain.FindNames(name));
            if (Problem != null)
                matches.AddRange(Problem.FindNames(name));
            return matches;
        }
    }
}
