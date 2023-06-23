using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Contextualisers
{
    public class PDDLDeclContextualiser : BaseContextualiser<PDDLDecl>
    {
        public override void Contexturalise(PDDLDecl decl, IErrorListener listener)
        {
            IContextualiser<DomainDecl> domainContextualiser = new PDDLDomainDeclContextualiser();
            domainContextualiser.Contexturalise(decl.Domain, listener);
            IContextualiser<ProblemDecl> problemContextualiser = new PDDLProblemDeclContextualiser();
            problemContextualiser.Contexturalise(decl.Problem, listener);

            SetDomainReference(decl.Problem, decl.Domain);
        }

        private void SetDomainReference(ProblemDecl problem, DomainDecl targetDomain)
        {
            if (problem.DomainName != null)
                problem.DomainName.DomainReference = targetDomain;
        }
    }
}
