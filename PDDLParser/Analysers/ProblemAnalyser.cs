using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public class ProblemAnalyser : IAnalyser<ProblemDecl>
    {
        public void PostAnalyse(ProblemDecl decl, IErrorListener listener)
        {
            CheckForBasicProblem(decl, listener);

            CheckForUniqueObjectNames(decl, listener);
            CheckDeclaredVsUsedObjects(decl, listener);
            CheckForUndeclaredObjects(decl, listener);

            CheckForValidGoal(decl, listener);
        }

        public void PreAnalyse(string file, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        private static void CheckForBasicProblem(ProblemDecl domain, IErrorListener listener)
        {
            if (domain.DomainName == null)
                listener.AddError(new ParseError(
                    $"Missing domain name reference",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Objects == null)
                listener.AddError(new ParseError(
                    $"Missing objects declaration",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Objects != null && domain.Objects.Objs.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing objects",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Init == null)
                listener.AddError(new ParseError(
                    $"Missing Init declaration",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Init != null && domain.Init.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No init predicates declared",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
            if (domain.Goal == null)
                listener.AddError(new ParseError(
                    $"Missing Goal declaration",
                    ParseErrorType.Message,
                    domain.Line,
                    domain.Character));
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
                    foreach (var init in problem.Init.Predicates)
                    {
                        foreach (var arg in init.Arguments)
                        {
                            if (!objects.Any(x => x.Name == arg.Name))
                            {
                                listener.AddError(new ParseError(
                                    $"Undeclared object detected!",
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
                    if (!objects.Any(x => x.Name == arg.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Undeclared object detected!",
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
            else if (exp is PredicateExp)
            {
                return true;
            }
            return false;
        }
    }
}
