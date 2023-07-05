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
    public class PDDLDomainDeclAnalyser : IAnalyser<DomainDecl>
    {
        public void PostAnalyse(DomainDecl decl, IErrorListener listener)
        {
            // Basics
            CheckForBasicDomain(decl, listener);

            // Declare Checking
            CheckForUndeclaredPredicates(decl, listener);
            CheckForUnusedPredicates(decl, listener);
            CheckForUnusedActionParameters(decl, listener);
            CheckForUnusedAxiomParameters(decl, listener);

            // Types
            CheckForUniqueTypeNames(decl, listener);
            BuildTypeTables(decl, listener);
            CheckForValidTypesInPredicates(decl, listener);
            CheckForValidTypesInConstants(decl, listener);
            CheckTypeMatchForActions(decl, listener);
            CheckTypeMatchForAxioms(decl, listener);

            // Unique Name Checking
            CheckForUniquePredicateNames(decl, listener);
            CheckForUniquePredicateParameterNames(decl, listener);
            CheckForUniqueActionNames(decl, listener);
            CheckForUniqueActionParameterNames(decl, listener);
            CheckForUniqueAxiomParameterNames(decl, listener);

            // Validity Checking
            CheckActionUsesValidPredicates(decl, listener);
            CheckAxiomUsesValidPredicates(decl, listener);
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
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No predicates defined.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
            if (domain.Actions == null)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
            if (domain.Actions != null && domain.Actions.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
        }

        private void CheckForUndeclaredPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                List<string> simplePredNames = new List<string>();
                foreach (var pred in domain.Predicates.Predicates)
                    simplePredNames.Add(pred.Name);
                var allPreds = domain.FindTypes<PredicateExp>();
                foreach(var pred in allPreds)
                {
                    if (!simplePredNames.Contains(pred.Name))
                        listener.AddError(new ParseError(
                            $"Undefined predicate! '{pred}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UseOfUndeclaredPredicate,
                            pred.Line,
                            pred.Start));
                }
            }
        }
        private void CheckForUnusedPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    if (domain.FindNames(predicate.Name).Count == 1)
                        listener.AddError(new ParseError(
                            $"Unused predicate detected '{predicate}'",
                            ParseErrorType.Message,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UnusedPredicate,
                            predicate.Line,
                            predicate.Start));
                }
            }
        }
        private void CheckForUnusedActionParameters(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                foreach (var act in domain.Actions)
                {
                    foreach (var param in act.Parameters)
                        if (act.FindNames(param.Name).Count == 0)
                            listener.AddError(new ParseError(
                                $"Unused action parameter found '{param.Name}'",
                                ParseErrorType.Message,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnusedParameter,
                                param.Line,
                                param.Start));
                }
            }
        }
        private void CheckForUnusedAxiomParameters(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null)
            {
                foreach (var axi in domain.Axioms)
                {
                    foreach (var param in axi.Vars)
                        if (axi.FindNames(param.Name).Count == 0)
                            listener.AddError(new ParseError(
                                $"Unused axiom variable found '{param.Name}'",
                                ParseErrorType.Message,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnusedParameter,
                                param.Line,
                                param.Start));
                }
            }
        }

        private void CheckForUniqueTypeNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Types != null)
            {
                List<string> declaredSubTypes = new List<string>();
                List<string> declaredSuperTypes = new List<string>();
                foreach (var typeDecl in domain.Types.Types)
                {
                    if (declaredSuperTypes.Contains(typeDecl.TypeName.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Multiple declarations of super types with the same name '{typeDecl.TypeName}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.MultipleDeclarationsOfSuperType,
                            typeDecl.Line,
                            typeDecl.Start));
                    }
                    declaredSuperTypes.Add(typeDecl.TypeName.Name);

                    foreach (var typeName in typeDecl.SubTypes)
                    {
                        if (declaredSubTypes.Contains(typeName.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Multiple declarations of sub types with the same name '{typeName}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.MultipleDeclarationsOfSubType,
                                typeDecl.Line,
                                typeDecl.Start));
                        }
                        declaredSubTypes.Add(typeName.Name);
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
                    if (domain.TypesTable.ContainsKey(typeDecl.TypeName.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Multiply defined types!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Contexturaliser,
                            ParserErrorCode.MultipleDeclarationsOfType,
                            typeDecl.Line,
                            typeDecl.Start));
                    }
                    else
                    {
                        var directName = new List<string>();
                        foreach (var typeName in typeDecl.SubTypes)
                            directName.Add(typeName.Name);
                        domain.TypesTable.Add(typeDecl.TypeName.Name, directName);
                    }
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
                        argTypeList.Add(arg.Type.Name);
                    if (domain.PredicateTypeTable.ContainsKey(pred.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Multiply defined predicates!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Contexturaliser,
                            ParserErrorCode.MultipleDeclarationsOfPredicate,
                            pred.Line,
                            pred.Start));
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
                        if (!domain.ContainsType(arg.Type.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Predicate arguments contains unknown type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnknownType,
                                arg.Line,
                                arg.Start));
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
                        if (!domain.ContainsType(param.Type.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnknownType,
                                param.Line,
                                param.Start));
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
                        if (!domain.ContainsType(param.Type.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnknownType,
                                param.Line,
                                param.Start));
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
                    if (!IsTypeOrSubType(arg.Type.Name, predicateTypeTable[pred.Name][index], typesTable))
                    {
                        listener.AddError(new ParseError(
                            $"Predicate has an invalid argument type! Expected a '{predicateTypeTable[pred.Name][index]}' but got a '{arg.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.TypeMissmatch,
                            arg.Line,
                            arg.Start));
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
                    if (!domain.ContainsType(cons.Type.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Constant contains unknown type!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UnknownType,
                            cons.Line,
                            cons.Start));
                    }
                }
            }
        }

        private void CheckForUniquePredicateNames(DomainDecl domain, IErrorListener listener)
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
                                ParserErrorCode.MultipleDeclarationsOfPredicate,
                                predicate.Line,
                                predicate.Start));
                    }
                    predicates.Add(predicate.Name);
                }
            }
        }
        private void CheckForUniquePredicateParameterNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    List<string> parameterNames = new List<string>();
                    foreach (var param in predicate.Arguments)
                    {
                        if (parameterNames.Contains(param.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Multiple declarations of arguments with the same name '{param.Name}' in the predicate '{predicate.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.MultipleDeclarationsOfParameter,
                                param.Line,
                                param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private void CheckForUniqueActionNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                List<string> actions = new List<string>();
                foreach (var act in domain.Actions)
                {
                    if (actions.Contains(act.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Multiple declarations of actions with the same name '{act.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.MultipleDeclarationsOfActions,
                            act.Line,
                            act.Start));
                    }
                    actions.Add(act.Name);
                }
            }
        }
        private void CheckForUniqueActionParameterNames(DomainDecl domain, IErrorListener listener)
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
                                    ParserErrorCode.MultipleDeclarationsOfParameter,
                                    param.Line,
                                    param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private void CheckForUniqueAxiomParameterNames(DomainDecl domain, IErrorListener listener)
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
                                    ParserErrorCode.MultipleDeclarationsOfParameter,
                                    param.Line,
                                    param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }

        private void CheckActionUsesValidPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null && domain.Predicates != null)
            {
                foreach (var action in domain.Actions)
                {
                    CheckExpUsesPredicates(action.Preconditions, domain.Predicates.Predicates, listener, domain.TypesTable);
                    CheckExpUsesPredicates(action.Effects, domain.Predicates.Predicates, listener, domain.TypesTable);
                }
            }
        }
        private void CheckAxiomUsesValidPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null && domain.Predicates != null)
            {
                foreach (var axiom in domain.Axioms)
                {
                    CheckExpUsesPredicates(axiom.Context, domain.Predicates.Predicates, listener, domain.TypesTable);
                    CheckExpUsesPredicates(axiom.Implies, domain.Predicates.Predicates, listener, domain.TypesTable);
                }
            }
        }
        private void CheckExpUsesPredicates(IExp node, List<PredicateExp> predicates, IErrorListener listener, Dictionary<string, List<string>> typeTable)
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
                            if (!IsTypeOrSubType(pred.Arguments[i].Type.Name, predicate.Arguments[i].Type.Name, typeTable))
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
                            ParserErrorCode.TypeMissmatch,
                            pred.Line,
                            pred.Start));
                    else
                        listener.AddError(new ParseError(
                            $"Undeclared predicate used '{pred.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UseOfUndeclaredPredicate,
                            pred.Line,
                            pred.Start));
                }
            }
        }
        private bool IsTypeOrSubType(string typeName, string targetType, Dictionary<string, List<string>> typeTable)
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
    }
}
