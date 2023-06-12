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
            foreach (var typeDecl in domain.Types.Types)
                foreach (var type in typeDecl.SubTypes)
                    declaredTypes.Add(type);

            // Check predicates
            foreach(var predicate in domain.Predicates.Predicates)
            {
                foreach(var arg in predicate.Arguments)
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

        public static void AnalyseProblem(ProblemDecl problem, IErrorListener listener)
        {

        }
    }
}
