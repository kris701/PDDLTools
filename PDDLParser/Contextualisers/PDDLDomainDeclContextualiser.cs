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
            LinkActionParameters(decl, listener);
            LinkAxiomVars(decl, listener);
        }

        private void LinkActionParameters(DomainDecl decl, IErrorListener listener)
        {
            if (decl.Actions != null)
            {
                foreach(var act in decl.Actions)
                {
                    foreach(var param in act.Parameters)
                    {
                        ReplaceNameExpWith(act.Preconditions, param);
                        ReplaceNameExpWith(act.Effects, param);
                    }
                }
            }
        }

        private void LinkAxiomVars(DomainDecl decl, IErrorListener listener)
        {
            if (decl.Axioms != null)
            {
                foreach (var axi in decl.Axioms)
                {
                    foreach (var param in axi.Vars)
                    {
                        ReplaceNameExpWith(axi.Context, param);
                        ReplaceNameExpWith(axi.Implies, param);
                    }
                }
            }
        }
    }
}
