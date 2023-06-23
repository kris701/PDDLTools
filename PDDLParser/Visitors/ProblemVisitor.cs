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
        public static IDecl Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("problem"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, "problem".Length).Trim();
                return new ProblemNameDecl(node, name);
            } 
            else if (node.Content.StartsWith(":domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, ":domain".Length).Trim();
                return new DomainNameRefDecl(node, name);
            } 
            else if (node.Content.StartsWith(":objects"))
            {
                DoesNodeHaveSpecificChildCount(node, ":objects", 0, listener);

                var parseStr = PurgeEscapeChars(node.Content.Replace(":objects", "")).Trim();
                var objs = LooseParseString(node, ":objects", parseStr, listener);

                return new ObjectsDecl(node, objs);
            }
            else if (node.Content.StartsWith(":init"))
            {
                var inits = ParseAsPredicateList(node, listener);
                return new InitDecl(node, inits);
            }
            else if (node.Content.StartsWith(":goal"))
            {
                DoesNodeHaveSpecificChildCount(node, ":goal", 1, listener);
                return new GoalDecl(node, ExpVisitor.Visit(node.Children[0], listener));
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }
    }
}
