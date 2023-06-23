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
            // Basics
            CheckForBasicDomain(decl, listener);

            // Declare Checking

            // Types
            CheckForUniqueTypeNames(decl, listener);
            BuildTypeTables(decl, listener);
            CheckForValidTypesInPredicates(decl, listener);
            CheckForValidTypesInConstants(decl, listener);
            CheckTypeMatchForActions(decl, listener);
            CheckTypeMatchForAxioms(decl, listener);

            // Unique Name Checking
            CheckForUniquePredicateNames(decl, listener);
            CheckForUniqueActionParameterNames(decl, listener);
            CheckForUniqueAxiomParameterNames(decl, listener);
            CheckActionUsesValidPredicates(decl, listener, decl.TypesTable);
            CheckAxiomUsesValidPredicates(decl, listener, decl.TypesTable);
            CheckForUnusedPredicates(decl, listener);
        }

        public void PreAnalyse(string text, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        private void CheckForBasicDomain(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates == null)
                listener.AddError(new ParseError(
                    $"Missing predicates declaration.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Character));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No predicates defined.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Character));
            if (domain.Actions == null)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Character));
            if (domain.Actions != null && domain.Actions.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Character));
        }

        private void CheckForUniqueTypeNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Types != null)
            {
                List<string> declaredSubTypes = new List<string>();
                List<string> declaredSuperTypes = new List<string>();
                foreach (var typeDecl in domain.Types.Types)
                {
                    if (declaredSuperTypes.Contains(typeDecl.TypeName))
                    {
                        listener.AddError(new ParseError(
                            $"Multiple declarations of super types with the same name '{typeDecl.TypeName}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            typeDecl.Line,
                            typeDecl.Character));
                    }
                    declaredSuperTypes.Add(typeDecl.TypeName);

                    foreach (var typeName in typeDecl.SubTypes)
                    {
                        if (declaredSubTypes.Contains(typeName))
                        {
                            listener.AddError(new ParseError(
                                $"Multiple declarations of sub types with the same name '{typeName}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                typeDecl.Line,
                                typeDecl.Character));
                        }
                        declaredSubTypes.Add(typeName);
                    }
                }
            }
        }
        private void BuildTypeTables(DomainDecl domain, IErrorListener listener)
        {
            domain.TypesTable = new Dictionary<string, List<string>>();
            if (domain.Types != null)
            {
                foreach (var typeDecl in domain.Types.Types)
                {
                    if (domain.TypesTable.ContainsKey(typeDecl.TypeName))
                    {
                        listener.AddError(new ParseError(
                            $"Multiply defined types!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Contexturaliser,
                            typeDecl.Line,
                            typeDecl.Character));
                    }
                    else
                        domain.TypesTable.Add(typeDecl.TypeName, typeDecl.SubTypes);
                }
            }
            if (!domain.TypesTable.ContainsKey(""))
                domain.TypesTable.Add("", new List<string>());
            if (!domain.TypesTable.ContainsKey("object"))
                domain.TypesTable.Add("object", new List<string>());

            if (domain.Predicates != null)
            {
                domain.PredicateTypeTable = new Dictionary<string, List<string>>();
                foreach (var pred in domain.Predicates.Predicates)
                {
                    var argTypeList = new List<string>();
                    foreach (var arg in pred.Arguments)
                        argTypeList.Add(arg.Type);
                    if (domain.PredicateTypeTable.ContainsKey(pred.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Multiply defined predicates!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Contexturaliser,
                            pred.Line,
                            pred.Character));
                    }
                    domain.PredicateTypeTable.Add(pred.Name, argTypeList);
                }
            }
        }
        private void CheckForValidTypesInPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                foreach(var predicate in domain.Predicates.Predicates)
                {
                    foreach(var arg in predicate.Arguments)
                    {
                        if (!domain.ContainsType(arg.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Predicate arguments contains unknown type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                arg.Line,
                                arg.Character));
                        }
                    }
                }
            }
        }
        private void CheckTypeMatchForActions(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                foreach(var act in domain.Actions)
                {
                    foreach(var param in act.Parameters)
                    {
                        if (!domain.ContainsType(param.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Character));
                        }
                    }

                    CheckForValidTypesInExp(act.Preconditions, domain.PredicateTypeTable, domain.TypesTable, listener);
                    CheckForValidTypesInExp(act.Effects, domain.PredicateTypeTable, domain.TypesTable, listener);
                }
            }
        }
        private void CheckTypeMatchForAxioms(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null)
            {
                foreach (var axi in domain.Axioms)
                {
                    foreach (var param in axi.Vars)
                    {
                        if (!domain.ContainsType(param.Type))
                        {
                            listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Character));
                        }
                    }

                    CheckForValidTypesInExp(axi.Context, domain.PredicateTypeTable, domain.TypesTable, listener);
                    CheckForValidTypesInExp(axi.Implies, domain.PredicateTypeTable, domain.TypesTable, listener);
                }
            }
        }
        private void CheckForValidTypesInExp(IExp node, Dictionary<string, List<string>> predicateTypeTable, Dictionary<string, List<string>> typesTable, IErrorListener listener)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForValidTypesInExp(child, predicateTypeTable, typesTable, listener);
            }
            else if (node is OrExp or)
            {
                CheckForValidTypesInExp(or.Option1, predicateTypeTable, typesTable, listener);
                CheckForValidTypesInExp(or.Option2, predicateTypeTable, typesTable, listener);
            }
            else if (node is NotExp not)
            {
                CheckForValidTypesInExp(not.Child, predicateTypeTable, typesTable, listener);
            }
            else if (node is PredicateExp pred)
            {
                int index = 0;
                foreach(var arg in pred.Arguments)
                {
                    if (!IsTypeOrSubType(arg.Type, predicateTypeTable[pred.Name][index], typesTable))
                    {
                        listener.AddError(new ParseError(
                            $"Predicate has an invalid argument type! Expected a '{predicateTypeTable[pred.Name][index]}' but got a '{arg.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            arg.Line,
                            arg.Character));
                    }
                    index++;
                }
            }
        }
        private void CheckForValidTypesInConstants(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Constants != null)
            {
                foreach(var cons in domain.Constants.Constants)
                {
                    if (!domain.ContainsType(cons.Type))
                    {
                        listener.AddError(new ParseError(
                            $"Constant contains unknown type!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            cons.Line,
                            cons.Character));
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
                                ParseErrorLevel.Analyser,
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
                                    ParseErrorLevel.Analyser,
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
                                    ParseErrorLevel.Analyser,
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
                            ParseErrorLevel.Analyser,
                            pred.Line,
                            pred.Character));
                    else
                        listener.AddError(new ParseError(
                            $"Undefined predicate used '{pred.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
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
                        ParseErrorLevel.Analyser,
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
