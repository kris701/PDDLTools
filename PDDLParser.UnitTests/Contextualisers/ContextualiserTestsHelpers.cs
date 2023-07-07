using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Tests.Contextualisers
{
    internal static class ContextualiserTestsHelpers
    {
        internal static bool AreAllNameExpOfType(IExp exp, string name, string type)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (!AreAllNameExpOfType(child, name, type))
                        return false;
            }
            else if (exp is OrExp or)
            {
                if (!AreAllNameExpOfType(or.Option1, name, type))
                    return false;
                if (!AreAllNameExpOfType(or.Option2, name, type))
                    return false;
            }
            else if (exp is NotExp not)
            {
                if (!AreAllNameExpOfType(not.Child, name, type))
                    return false;
            }
            else if (exp is PredicateExp pred)
            {
                foreach (var arg in pred.Arguments)
                    if (!AreAllNameExpOfType(arg, name, type))
                        return false;
            }
            else if (exp is NameExp nameExp)
            {
                if (nameExp.Name == name)
                {
                    if (nameExp.Type.Name != type)
                        return false;
                }
            }
            return true;
        }
    }
}
