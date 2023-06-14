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
using System.Xml.Linq;

namespace PDDLParser.Analysers
{
    public static class PostParsingAnalyser
    {
        public static void AnalyseDomain(DomainDecl domain, IErrorListener listener)
        {
            Dictionary<string, List<string>> typeTable = new Dictionary<string, List<string>>();
            if (domain.Types != null)
            {
                foreach (var typeDecl in domain.Types.Types)
                    typeTable.Add(typeDecl.TypeName, typeDecl.SubTypes);
            }

            CheckForBasicDomain(domain, listener);

            CheckDeclaredVsUsedTypes(domain, listener);
            CheckForUniquePredicateNames(domain, listener);
            CheckForUniqueActionParameterNames(domain, listener);
            CheckForUniqueAxiomParameterNames(domain, listener);
            CheckActionUsesValidPredicates(domain, listener, typeTable);
            CheckForUnusedPredicates(domain, listener);
        }

        private static void CheckForBasicDomain(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates == null)
                listener.AddError(new ParseError(
                    $"Missing predicates declaration.",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No predicates defined.",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Actions == null)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Actions != null && domain.Actions.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParserErrorLevel.High,
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
                    foreach (var constant in domain.Constants.Constants)
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
                        foreach (var param in action.Parameters)
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
        private static void CheckActionUsesValidPredicates(DomainDecl domain, IErrorListener listener, Dictionary<string, List<string>> typeTable)
        {
            if (domain.Actions != null && domain.Predicates != null)
            {
                List<PredicateExp> predicates = new List<PredicateExp>();
                if (domain.Predicates != null)
                    foreach (var predicate in domain.Predicates.Predicates)
                        predicates.Add(predicate);

                foreach(var action in domain.Actions)
                {
                    CheckExpUsesPredicates(action.Preconditions, predicates, listener, typeTable);
                    CheckExpUsesPredicates(action.Effects, predicates, listener, typeTable);
                }
            }
        }
        private static void CheckExpUsesPredicates(IExp node, List<PredicateExp> predicates, IErrorListener listener, Dictionary<string, List<string>> typeTable)
        {
            if (node is AndExp and)
            {
                foreach(var child in and.Children)
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
                foreach(var predicate in predicates)
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
                            ParserErrorLevel.High,
                            ParseErrorType.Error,
                            pred.Line,
                            pred.Character));
                    else
                        listener.AddError(new ParseError(
                            $"Undefined predicate used '{pred.Name}'",
                            ParserErrorLevel.High,
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
                                    i = 0;
                                    continue;
                                }
                            }

                            if (action.Effects != null)
                            {
                                if (IsPredicateUsed(action.Effects, predicates[i].Name))
                                {
                                    predicates.RemoveAt(i);
                                    i = 0;
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
                                    i = 0;
                                    continue;
                                }
                            }
                            if (axiom.Implies != null)
                            {
                                if (IsPredicateUsed(axiom.Implies, predicates[i].Name))
                                {
                                    predicates.RemoveAt(i);
                                    i = 0;
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
                        ParserErrorLevel.Medium,
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

        public static void AnalyseProblem(ProblemDecl problem, IErrorListener listener)
        {
            CheckForBasicProblem(problem, listener);

            CheckForAnyObjects(problem, listener);
            CheckForUniqueObjectNames(problem, listener);
            CheckDeclaredVsUsedObjects(problem, listener);
            CheckForUndeclaredObjects(problem, listener);

            CheckForValidGoal(problem, listener);
        }

        private static void CheckForBasicProblem(ProblemDecl domain, IErrorListener listener)
        {
            if (domain.DomainName == null)
                listener.AddError(new ParseError(
                    $"Missing domain name reference",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Objects == null)
                listener.AddError(new ParseError(
                    $"Missing objects declaration",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Objects != null && domain.Objects.Objs.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing objects",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Init == null)
                listener.AddError(new ParseError(
                    $"Missing Init declaration",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Init != null && domain.Init.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No init predicates declared",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Goal == null)
                listener.AddError(new ParseError(
                    $"Missing Goal declaration",
                    ParserErrorLevel.High,
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
        }
        private static void CheckForAnyObjects(ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Objects != null)
            {
                if (problem.Objects.Objs.Count == 0)
                {
                    listener.AddError(new ParseError(
                        $"No objects declared!",
                        ParserErrorLevel.High,
                        ParseErrorType.Warning,
                        problem.Objects.Line,
                        problem.Objects.Character));
                }
            }
        }
        private static void CheckForUndeclaredObjects(ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Objects != null)
            {
                List<NameExp> objects = new List<NameExp>();
                foreach (var obj in problem.Objects.Objs)
                    objects.Add(obj.Clone() as NameExp);

                if (problem.Init != null)
                {
                    foreach(var init in problem.Init.Predicates)
                    {
                        foreach(var arg in init.Arguments)
                        {
                            if (!objects.Contains(arg))
                            {
                                listener.AddError(new ParseError(
                                    $"Undeclared object detected!",
                                    ParserErrorLevel.High,
                                    ParseErrorType.Error,
                                    arg.Line,
                                    arg.Character));
                            }
                        }
                    }
                }

                if (problem.Goal != null)
                    CheckForUndeclaredExpObjects(problem.Goal.GoalExp, objects, listener);
            }
        }
        private static void CheckForUndeclaredExpObjects(IExp exp, List<NameExp> objects, IErrorListener listener)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForUndeclaredExpObjects(child, objects, listener);
            }
            else if (exp is OrExp or)
            {
                CheckForUndeclaredExpObjects(or.Option1, objects, listener);
                CheckForUndeclaredExpObjects(or.Option2, objects, listener);
            }
            else if (exp is NotExp not)
            {
                CheckForUndeclaredExpObjects(not.Child, objects, listener);
            }
            else if (exp is PredicateExp pred)
            {
                foreach (var arg in pred.Arguments)
                {
                    if (!objects.Contains(arg))
                    {
                        listener.AddError(new ParseError(
                            $"Undeclared object detected!",
                            ParserErrorLevel.High,
                            ParseErrorType.Error,
                            arg.Line,
                            arg.Character));
                    }
                }
            }
        }
        private static void CheckForUniqueObjectNames(ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Objects != null)
            {
                List<string> objs = new List<string>();
                foreach (var obj in problem.Objects.Objs)
                {
                    if (objs.Contains(obj.Name))
                    {
                        listener.AddError(new ParseError(
                                $"Multiple declarations of object with the same name '{obj.Name}'",
                                ParserErrorLevel.High,
                                ParseErrorType.Error,
                                obj.Line,
                                obj.Character));
                    }
                    objs.Add(obj.Name);
                }
            }
        }
        private static void CheckDeclaredVsUsedObjects(ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Objects != null && problem.Init != null)
            {
                foreach (var obj in problem.Objects.Objs)
                {
                    bool isFound = false;
                    foreach (var init in problem.Init.Predicates)
                    {
                        foreach (var arg in init.Arguments)
                        {
                            if (arg.Name == obj.Name)
                            {
                                isFound = true;
                                break;
                            }
                        }
                        if (isFound)
                            break;
                    }

                    if (!isFound)
                        listener.AddError(new ParseError(
                            $"Unused object detected '{obj.Name}'",
                            ParserErrorLevel.Medium,
                            ParseErrorType.Message,
                            obj.Line,
                            obj.Character));
                }
            }
        }
        private static void CheckForValidGoal(ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Goal != null)
            {
                if (!DoesAnyPredicatesExist(problem.Goal.GoalExp))
                {
                    listener.AddError(new ParseError(
                        $"No actual goal predicates in the goal declaration!",
                        ParserErrorLevel.Medium,
                        ParseErrorType.Warning,
                        problem.Goal.GoalExp.Line,
                        problem.Goal.GoalExp.Character));
                }
            }
        }
        private static bool DoesAnyPredicatesExist(IExp exp)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (DoesAnyPredicatesExist(child))
                        return true;
            }
            else if (exp is OrExp or)
            {
                if (DoesAnyPredicatesExist(or.Option1))
                    return true;
                if (DoesAnyPredicatesExist(or.Option2))
                    return true;
            }
            else if (exp is NotExp not)
            {
                return DoesAnyPredicatesExist(not.Child);
            }
            else if (exp is PredicateExp pred)
            {
                return true;
            }
            return false;
        }
    }
}
