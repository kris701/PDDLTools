using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser
{
    public interface IPDDLParser
    {
        DomainDecl ParseDomainFile(string parseFile);
        ProblemDecl ParseProblemFile(string parseFile);
        PDDLDecl ParseDomainAndProblemFiles(string domainFile, string problemFile);
    }
}
