using PDDLParser.Listener;
using PDDLParser.Models;
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
            CheckForUniquePredicateNames(domain, listener);
            CheckForUniqueActionParameterNames(domain, listener);
            CheckForUniqueAxiomParameterNames(domain, listener);
            CheckActionUsesValidPredicates(domain, listener);
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
        private static void CheckForUniquePredicateNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                List<string> predicates = new List<string>();
                foreach(var predicate in domain.Predicates.Predicates)
                {
                    if (predicates.Contains(predicate.Name))
                    {
                        listener.AddError(new ParseError(
                                $"Multiple declarations of predicates with the same name '{predicate.Name}'",
                                ParserErrorLevel.High,
                                ParseErrorType.Error,
                                predicate.Line,
                                predicate.Character));
                    }
                    predicates.Add(predicate.Name);
                }
            }
        }
        private static void CheckForUniqueActionParameterNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                foreach (var action in domain.Actions)
                {
                    List<string> parameterNames = new List<string>();
                    foreach (var param in action.Parameters)
                    {
                        if (parameterNames.Contains(param.Name))
                        {
                            listener.AddError(new ParseError(
                                    $"Multiple declarations of arguments with the same name '{param.Name}' in the action '{action.Name}'",
                                    ParserErrorLevel.High,
                                    ParseErrorType.Error,
                                    param.Line,
                                    param.Character));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private static void CheckForUniqueAxiomParameterNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null)
            {
                foreach (var axiom in domain.Axioms)
                {
                    List<string> parameterNames = new List<string>();
                    foreach (var param in axiom.Vars)
                    {
                        if (parameterNames.Contains(param.Name))
                        {
                            listener.AddError(new ParseError(
                                    $"Multiple declarations of arguments with the same name '{param.Name}' in axiom",
                                    ParserErrorLevel.High,
                                    ParseErrorType.Error,
                                    param.Line,
                                    param.Character));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private static void CheckActionUsesValidPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                List<string> predicateNames = new List<string>();
                if (domain.Predicates != null)
                    foreach (var predicate in domain.Predicates.Predicates)
                        predicateNames.Add(predicate.Name);

                foreach(var action in domain.Actions)
                {
                    CheckExpUsesPredicates(action.Preconditions, predicateNames, listener);
                    CheckExpUsesPredicates(action.Effects, predicateNames, listener);
                }
            }
        }
        private static void CheckExpUsesPredicates(IExp node, List<string> predicateNames, IErrorListener listener)
        {
            if (node is AndExp and)
            {
                foreach(var child in and.Children)
                    CheckExpUsesPredicates(child, predicateNames, listener);
            }
            else if (node is OrExp or)
            {
                CheckExpUsesPredicates(or.Option1, predicateNames, listener);
                CheckExpUsesPredicates(or.Option2, predicateNames, listener);
            }
            else if (node is NotExp not)
            {
                CheckExpUsesPredicates(not.Child, predicateNames, listener);
            }
            else if (node is NameExp name)
            {
                if (!predicateNames.Contains(name.Name))
                {
                    listener.AddError(new ParseError(
                        $"Undefined predicate used '{name.Name}'",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        name.Line,
                        name.Character));
                }
            }
        }

        public static void AnalyseProblem(ProblemDecl problem, IErrorListener listener)
        {

        }
    }
}
