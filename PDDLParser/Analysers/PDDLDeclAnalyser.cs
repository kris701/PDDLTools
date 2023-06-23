using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public class PDDLDeclAnalyser : IAnalyser<PDDLDecl>
    {
        public void PostAnalyse(PDDLDecl decl, IErrorListener listener)
        {
            IAnalyser<ProblemDecl> problemAnalyser = new ProblemAnalyser();
            problemAnalyser.PostAnalyse(decl.Problem, listener);
            IAnalyser<DomainDecl> domainAnalyser = new DomainAnalyser();
            domainAnalyser.PostAnalyse(decl.Domain, listener);
        }

        public void PreAnalyse(string text, IErrorListener listener)
        {
            throw new NotImplementedException();
        }
    }
}
