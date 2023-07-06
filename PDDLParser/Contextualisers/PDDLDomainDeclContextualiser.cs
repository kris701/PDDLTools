using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Contextualisers
{
    public class PDDLDomainDeclContextualiser : BaseContextualiser<DomainDecl>
    {
        public override void Contexturalise(DomainDecl decl, IErrorListener listener)
        {
            DecorateActionParameters(decl, listener);
            DecorateAxiomVars(decl, listener);
        }

        private void DecorateActionParameters(DomainDecl decl, IErrorListener listener)
        {
            if (decl.Actions != null)
            {
                foreach(var act in decl.Actions)
                {
                    foreach(var param in act.Parameters)
                    {
                        ReplaceNameExpTypeWith(act.Preconditions, param);
                        ReplaceNameExpTypeWith(act.Effects, param);
                    }
                }
            }
        }

        private void DecorateAxiomVars(DomainDecl decl, IErrorListener listener)
        {
            if (decl.Axioms != null)
            {
                foreach (var axi in decl.Axioms)
                {
                    foreach (var param in axi.Vars)
                    {
                        ReplaceNameExpTypeWith(axi.Context, param);
                        ReplaceNameExpTypeWith(axi.Implies, param);
                    }
                }
            }
        }
    }
}
