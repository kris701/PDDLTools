using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Visitors
{
    public class ProblemExpVisitor
    {
        public static IExp Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("and"))
            {
                if (node.Children.Count == 0)
                    listener.AddError(new ParseError(
                        $"'and' node does not have any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error));
                IsChildrenOnly(node, "and", listener);

                List<IExp> children = new List<IExp>();
                foreach(var child in node.Children)
                    children.Add(Visit(child, listener));

                return new AndExp(node, children);
            } else if (node.Content.StartsWith("or"))
            {
                if (node.Children.Count != 0)
                    listener.AddError(new ParseError(
                        $"'or' node must have exactly 2 children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error));
                IsChildrenOnly(node, "or", listener);

                return new OrExp(node, Visit(node.Children[0], listener), Visit(node.Children[1], listener));
            }
            else if (node.Content.StartsWith("not"))
            {
                if (node.Children.Count == 0)
                    listener.AddError(new ParseError(
                        $"'not' node does not have any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (node.Children.Count > 1)
                    listener.AddError(new ParseError(
                        $"'not' node should only have one child!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                IsChildrenOnly(node, "not", listener);

                return new NotExp(node, Visit(node.Children[0], listener));
            }
            else if (node.Content.Contains(" "))
            {
                if (node.Children.Count > 0)
                    listener.AddError(new ParseError(
                        $"Predicate experssion does not expect any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));

                var predicateName = node.Content.Split(' ')[0];
                List<NameExp> parameters = new List<NameExp>();

                var paramSplit = node.Content.Split(' ');
                foreach (var param in paramSplit)
                    if (param != "" && param != predicateName)
                        parameters.Add(Visit(new ASTNode(node.Character, node.Line, param), listener) as NameExp);
                return new PredicateExp(node, predicateName, parameters);
            }
            else if (node.Content.Contains(ASTTokens.TypeToken))
            {
                if (node.Children.Count > 0)
                    listener.AddError(new ParseError(
                        $"Name experssion does not expect any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));

                var left = node.Content.Substring(0, node.Content.IndexOf(ASTTokens.TypeToken)).Trim();
                var right = node.Content.Substring(node.Content.IndexOf(ASTTokens.TypeToken) + 3).Trim();

                if (left == "")
                {
                    listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but an object name was not given!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                }
                if (right == "")
                {
                    listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but a type was not given!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                }

                return new NameExp(node, left.Replace("?", ""), right);
            }
            else
            {
                if (node.Content.Trim() == "")
                    listener.AddError(new ParseError(
                        $"Invalid node detected!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));

                if (node.Children.Count > 0)
                    listener.AddError(new ParseError(
                        $"Name experssion does not expect any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));

                return new NameExp(node, node.Content.Replace("?","").Trim());
            }
        }

        private static void IsChildrenOnly(ASTNode node, string targetName, IErrorListener listener)
        {
            if (node.Content.Replace(targetName, "").Trim() != "")
                listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.Content.Replace(targetName, "").Trim()}",
                    ParserErrorLevel.High,
                    ParseErrorType.Error,
                    node.Line,
                    node.Character));
        }
    }
}
