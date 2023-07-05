using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public class PDDLDeclAnalyser : IAnalyser<PDDLDecl>
    {
        public void PostAnalyse(PDDLDecl decl, IErrorListener listener)
        {
            IAnalyser<ProblemDecl> problemAnalyser = new PDDLProblemDeclAnalyser();
            problemAnalyser.PostAnalyse(decl.Problem, listener);
            IAnalyser<DomainDecl> domainAnalyser = new PDDLDomainDeclAnalyser();
            domainAnalyser.PostAnalyse(decl.Domain, listener);

            // Declare Checking
            CheckForUndeclaredPreconditionsInInits(decl.Domain, decl.Problem, listener);
            CheckForUndeclaredPreconditionsInGoal(decl.Domain, decl.Problem, listener);

            // Types
            CheckObjectDeclarationTypes(decl.Domain, decl.Problem, listener);
            CheckForInitDeclarationTypes(decl.Domain, decl.Problem, listener);
            CheckForGoalDeclarationTypes(decl.Domain, decl.Problem, listener);
        }

        public void PreAnalyse(string text, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        private void CheckForUndeclaredPreconditionsInInits(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Init != null && domain.Predicates != null)
            {
                foreach (var init in problem.Init.Predicates)
                    if (!domain.Predicates.Predicates.Any(x => x.Name == init.Name))
                        listener.AddError(new ParseError(
                            $"Used of undeclared predicate in problem '{init.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UseOfUndeclaredPredicate,
                            init.Line,
                            init.Start));
            }
        }
        private void CheckForUndeclaredPreconditionsInGoal(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Goal != null && domain.Predicates != null)
                DoesExpContainValidPredicates(problem.Goal.GoalExp, domain.Predicates.Predicates, listener);
        }
        private void DoesExpContainValidPredicates(IExp exp, List<PredicateExp> predicates, IErrorListener listener)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    DoesExpContainValidPredicates(child, predicates, listener);
            }
            else if (exp is OrExp or)
            {
                DoesExpContainValidPredicates(or.Option1, predicates, listener);
                DoesExpContainValidPredicates(or.Option2, predicates, listener);
            }
            else if (exp is NotExp not)
            {
                DoesExpContainValidPredicates(not.Child, predicates, listener);
            }
            else if (exp is PredicateExp pred)
            {
                if (!predicates.Any(x => x.Name == pred.Name))
                    listener.AddError(new ParseError(
                        $"Used of undeclared predicate in expression '{pred.Name}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        ParserErrorCode.UseOfUndeclaredPredicate,
                        pred.Line,
                        pred.Start));
            }
        }

        private void CheckObjectDeclarationTypes(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Objects != null)
            {
                foreach(var obj in problem.Objects.Objs)
                    if (!domain.ContainsType(obj.Type))
                        listener.AddError(new ParseError(
                            $"Unknown type for object! '{obj.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.InvalidObjectType,
                            obj.Line,
                            obj.Start));
            }
        }
        private void CheckForInitDeclarationTypes(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Init != null && domain.Predicates != null)
            {
                foreach(var init in problem.Init.Predicates)
                {
                    var target = domain.Predicates.Predicates.Single(x => x.Name == init.Name);
                    for(int i = 0; i < init.Arguments.Count; i++)
                    {
                        if (!domain.IsTypeOrSubType(init.Arguments[i].Type, target.Arguments[i].Type))
                            listener.AddError(new ParseError(
                                $"Invalid type for init precondition! Got '{init.Arguments[i].Type}' but expected '{target.Arguments[i].Type}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.InvalidPredicateType,
                                init.Line,
                                init.Start));
                    }
                }
            }
        }
        private void CheckForGoalDeclarationTypes(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Goal != null && domain.Predicates != null)
                CheckExpUsesPredicates(problem.Goal.GoalExp, domain.Predicates.Predicates, listener, domain);
        }
        private void CheckExpUsesPredicates(IExp node, List<PredicateExp> predicates, IErrorListener listener, DomainDecl domain)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckExpUsesPredicates(child, predicates, listener, domain);
            }
            else if (node is OrExp or)
            {
                CheckExpUsesPredicates(or.Option1, predicates, listener, domain);
                CheckExpUsesPredicates(or.Option2, predicates, listener, domain);
            }
            else if (node is NotExp not)
            {
                CheckExpUsesPredicates(not.Child, predicates, listener, domain);
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
                            if (!domain.IsTypeOrSubType(pred.Arguments[i].Type, predicate.Arguments[i].Type))
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
    }
}
