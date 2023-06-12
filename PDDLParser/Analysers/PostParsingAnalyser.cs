using PDDLParser.Listener;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public static class PostParsingAnalyser
    {
        public static void AnalyseDomain(DomainDecl domain, IErrorListener listener)
        {
            CheckDeclaredVsUsedTypes(domain, listener);
        }

        private static void CheckDeclaredVsUsedTypes(DomainDecl domain, IErrorListener listener)
        {
            List<string> declaredTypes = new List<string>();
            // Default in PDDL
            declaredTypes.Add("object");
            declaredTypes.Add("");
            if (domain.Types != null)
                foreach (var typeDecl in domain.Types.Types)
                    foreach (var type in typeDecl.SubTypes)
                        declaredTypes.Add(type);

            // Check predicates
            if (domain.Predicates != null)
            {
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    foreach (var arg in predicate.Arguments)
                    {
                        if (!declaredTypes.Contains(arg.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Use of undeclared type '{arg.Type}'",
                                ParserErrorLevel.High,
                                ParseErrorType.Error,
                                arg.Line,
                                arg.Character));
                        }
                    }
                }
            }

            // Check constants
            if (domain.Constants != null)
            {
                foreach(var constant in domain.Constants.Constants)
                {
                    if (!declaredTypes.Contains(constant.Type))
                    {
                        listener.AddError(new ParseError(
                            $"Use of undeclared type '{constant.Type}'",
                            ParserErrorLevel.High,
                            ParseErrorType.Error,
                            constant.Line,
                            constant.Character));
                    }
                }
            }

            // Check actions
            if (domain.Actions != null)
            {
                foreach (var action in domain.Actions)
                {
                    foreach(var param in action.Parameters)
                    {
                        if (!declaredTypes.Contains(param.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Use of undeclared type '{param.Type}'",
                                ParserErrorLevel.High,
                                ParseErrorType.Error,
                                param.Line,
                                param.Character));
                        }
                    }
                }
            }

            // Check axioms
            if (domain.Axioms != null)
            {
                foreach (var axiom in domain.Axioms)
                {
                    foreach (var variable in axiom.Vars)
                    {
                        if (!declaredTypes.Contains(variable.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Use of undeclared type '{variable.Type}'",
                                ParserErrorLevel.High,
                                ParseErrorType.Error,
                                variable.Line,
                                variable.Character));
                        }
                    }
                }
            }
        }

        public static void AnalyseProblem(ProblemDecl problem, IErrorListener listener)
        {

        }
    }
}
