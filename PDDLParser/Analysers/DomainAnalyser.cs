using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public class DomainAnalyser : IAnalyser<DomainDecl>
    {
        public void PostAnalyse(DomainDecl decl, IErrorListener listener)
        {
            Dictionary<string, List<string>> typeTable = new Dictionary<string, List<string>>();
            if (decl.Types != null)
            {
                foreach (var typeDecl in decl.Types.Types)
                    typeTable.Add(typeDecl.TypeName, typeDecl.SubTypes);
            }

            CheckForBasicDomain(decl, listener);

            CheckDeclaredVsUsedTypes(decl, listener);
            CheckForUniquePredicateNames(decl, listener);
            CheckForUniqueActionParameterNames(decl, listener);
            CheckForUniqueAxiomParameterNames(decl, listener);
            CheckActionUsesValidPredicates(decl, listener, typeTable);
            CheckAxiomUsesValidPredicates(decl, listener, typeTable);
            CheckForUnusedPredicates(decl, listener);
        }

        public void PreAnalyse(string file, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        private static void CheckForBasicDomain(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates == null)
                listener.AddError(new ParseError(
                    $"Missing predicates declaration.",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No predicates defined.",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Actions == null)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Actions != null && domain.Actions.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
        }

        private static void CheckDeclaredVsUsedTypes(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Types != null)
            {
                List<string> declaredTypes = new List<string>();
                // Default in PDDL
                declaredTypes.Add("object");
                declaredTypes.Add("");

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
                    foreach (var constant in domain.Constants.Constants)
                    {
                        if (!declaredTypes.Contains(constant.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Use of undeclared type '{constant.Type}'",
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
                        foreach (var param in action.Parameters)
                        {
                            if (!declaredTypes.Contains(param.Type))
                            {
                                listener.AddError(new ParseError(
                                    $"Use of undeclared type '{param.Type}'",
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
                                    ParseErrorType.Error,
                                    variable.Line,
                                    variable.Character));
                            }
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
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    if (predicates.Contains(predicate.Name))
                    {
                        listener.AddError(new ParseError(
                                $"Multiple declarations of predicates with the same name '{predicate.Name}'",
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
                                    ParseErrorType.Error,
                                    param.Line,
                                    param.Character));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private static void CheckActionUsesValidPredicates(DomainDecl domain, IErrorListener listener, Dictionary<string, List<string>> typeTable)
        {
            if (domain.Actions != null && domain.Predicates != null)
            {
                List<PredicateExp> predicates = new List<PredicateExp>();
                if (domain.Predicates != null)
                    foreach (var predicate in domain.Predicates.Predicates)
                        predicates.Add(predicate);

                foreach (var action in domain.Actions)
                {
                    CheckExpUsesPredicates(action.Preconditions, predicates, listener, typeTable);
                    CheckExpUsesPredicates(action.Effects, predicates, listener, typeTable);
                }
            }
        }
        private static void CheckAxiomUsesValidPredicates(DomainDecl domain, IErrorListener listener, Dictionary<string, List<string>> typeTable)
        {
            if (domain.Axioms != null && domain.Predicates != null)
            {
                List<PredicateExp> predicates = new List<PredicateExp>();
                if (domain.Predicates != null)
                    foreach (var predicate in domain.Predicates.Predicates)
                        predicates.Add(predicate);

                foreach (var axiom in domain.Axioms)
                {
                    CheckExpUsesPredicates(axiom.Context, predicates, listener, typeTable);
                    CheckExpUsesPredicates(axiom.Implies, predicates, listener, typeTable);
                }
            }
        }
        private static void CheckExpUsesPredicates(IExp node, List<PredicateExp> predicates, IErrorListener listener, Dictionary<string, List<string>> typeTable)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckExpUsesPredicates(child, predicates, listener, typeTable);
            }
            else if (node is OrExp or)
            {
                CheckExpUsesPredicates(or.Option1, predicates, listener, typeTable);
                CheckExpUsesPredicates(or.Option2, predicates, listener, typeTable);
            }
            else if (node is NotExp not)
            {
                CheckExpUsesPredicates(not.Child, predicates, listener, typeTable);
            }
            else if (node is PredicateExp pred)
            {
                bool any = false;
                bool wasTypeMissmatch = false;
                foreach (var predicate in predicates)
                {
                    if (predicate.Name == pred.Name && predicate.Arguments.Count == pred.Arguments.Count)
                    {
                        any = true;
                        for (int i = 0; i < predicate.Arguments.Count; i++)
                        {
                            if (!IsTypeOrSubType(pred.Arguments[i].Type, predicate.Arguments[i].Type, typeTable))
                            {
                                wasTypeMissmatch = true;
                                any = false;
                                break;
                            }
                        }
                    }
                }
                if (!any)
                {
                    if (wasTypeMissmatch)
                        listener.AddError(new ParseError(
                            $"Used predicate '{pred.Name}' did not match the type definitions from the parameters!",
                            ParseErrorType.Error,
                            pred.Line,
                            pred.Character));
                    else
                        listener.AddError(new ParseError(
                            $"Undefined predicate used '{pred.Name}'",
                            ParseErrorType.Error,
                            pred.Line,
                            pred.Character));
                }
            }
        }
        private static bool IsTypeOrSubType(string typeName, string targetType, Dictionary<string, List<string>> typeTable)
        {
            if (typeName == targetType)
                return true;

            if (typeTable.ContainsKey(targetType))
            {
                if (typeTable[targetType].Contains(typeName))
                    return true;
            }

            return false;
        }
        private static void CheckForUnusedPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                List<PredicateExp> predicates = new List<PredicateExp>();
                foreach (var predicate in domain.Predicates.Predicates)
                    predicates.Add(predicate.Clone() as PredicateExp);

                if (domain.Actions != null)
                {
                    foreach (var action in domain.Actions)
                    {
                        for (int i = 0; i < predicates.Count; i++)
                        {
                            if (action.Preconditions != null)
                            {
                                if (IsPredicateUsed(action.Preconditions, predicates[i].Name))
                                {
                                    predicates.RemoveAt(i);
                                    i = -1;
                                    continue;
                                }
                            }

                            if (action.Effects != null)
                            {
                                if (IsPredicateUsed(action.Effects, predicates[i].Name))
                                {
                                    predicates.RemoveAt(i);
                                    i = -1;
                                    continue;
                                }
                            }
                        }
                    }
                }

                if (domain.Axioms != null)
                {
                    foreach (var axiom in domain.Axioms)
                    {
                        for (int i = 0; i < predicates.Count; i++)
                        {
                            if (axiom.Context != null)
                            {
                                if (IsPredicateUsed(axiom.Context, predicates[i].Name))
                                {
                                    predicates.RemoveAt(i);
                                    i = -1;
                                    continue;
                                }
                            }
                            if (axiom.Implies != null)
                            {
                                if (IsPredicateUsed(axiom.Implies, predicates[i].Name))
                                {
                                    predicates.RemoveAt(i);
                                    i = -1;
                                    continue;
                                }
                            }
                        }
                    }
                }

                foreach (var predicate in predicates)
                {
                    listener.AddError(new ParseError(
                        $"Unused predicate detected '{predicate}'",
                        ParseErrorType.Message,
                        predicate.Line,
                        predicate.Character));
                }
            }
        }
        private static bool IsPredicateUsed(IExp exp, string predicate)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (IsPredicateUsed(child, predicate))
                        return true;
            }
            else if (exp is OrExp or)
            {
                if (IsPredicateUsed(or.Option1, predicate))
                    return true;
                if (IsPredicateUsed(or.Option2, predicate))
                    return true;
            }
            else if (exp is NotExp not)
            {
                return IsPredicateUsed(not.Child, predicate);
            }
            else if (exp is PredicateExp pred)
            {
                if (pred.Name == predicate)
                    return true;
            }
            return false;
        }

    }
}
