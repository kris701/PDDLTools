using PDDLParser.AST;
using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Visitors
{
    public class ProblemVisitor : BaseVisitor
    {
        public static IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            if (node.Content.StartsWith("problem"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, "problem".Length).Trim();
                return new ProblemNameDecl(node, parent, name);
            } 
            else if (node.Content.StartsWith(":domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, ":domain".Length).Trim();
                return new DomainNameRefDecl(node, parent, name);
            } 
            else if (node.Content.StartsWith(":objects"))
            {
                DoesNodeHaveSpecificChildCount(node, ":objects", 0, listener);

                var newObjs = new ObjectsDecl(node, parent, new List<NameExp>());

                var parseStr = PurgeEscapeChars(node.Content.Replace(":objects", "")).Trim();
                newObjs.Objs = LooseParseString(node, newObjs, ":objects", parseStr, listener);

                return newObjs;
            }
            else if (node.Content.StartsWith(":init"))
            {
                var newInit = new InitDecl(node, parent, new List<PredicateExp>());
                newInit.Predicates = ParseAsPredicateList(node, newInit, listener);
                return newInit;
            }
            else if (node.Content.StartsWith(":goal"))
            {
                DoesNodeHaveSpecificChildCount(node, ":goal", 1, listener);
                var newGoal = new GoalDecl(node, parent, null);
                newGoal.GoalExp = ExpVisitor.Visit(node.Children[0], newGoal, listener);
                return newGoal;
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }
    }
}
