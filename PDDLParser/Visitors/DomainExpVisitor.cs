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
    public class DomainExpVisitor
    {
        public static IExp Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("and"))
            {
                List<IExp> children = new List<IExp>();
                foreach(var child in node.Children)
                    children.Add(Visit(child, listener));
                if (children.Count == 0)
                {
                    listener.AddError(new ParseError(
                        $"'and' node does not have any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error));
                }
                return new AndExp(node, children);
            } else if (node.Content.StartsWith("or"))
            {
                if (node.Children.Count != 0)
                {
                    listener.AddError(new ParseError(
                        $"'or' node must have exactly 2 children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error));
                }
                return new OrExp(node, Visit(node.Children[0], listener), Visit(node.Children[1], listener));
            }
            else if (node.Content.StartsWith("not"))
            {
                if (node.Children.Count == 0)
                {
                    listener.AddError(new ParseError(
                        $"'not' node does not have any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                }
                return new NotExp(node, Visit(node.Children[0], listener));
            } 
            else if (node.Content.Contains("?"))
            {
                var predicateName = node.Content.Split(' ')[0];
                var parseStr = node.Content.Remove(0,predicateName.Length).Trim();
                List<NameExp> parameters = new List<NameExp>();

                var paramSplit = parseStr.Split('?');
                foreach (var param in paramSplit)
                    if (param != "")
                        parameters.Add(Visit(new ASTNode(node.Character, node.Line, param), listener) as NameExp);
                foreach (var param in parameters)
                    param.Name = $"?{param.Name}";
                return new PredicateExp(node, predicateName, parameters);
            }
            else
            {
                if (node.Content.Contains(" - "))
                {
                    var left = node.Content.Substring(0, node.Content.IndexOf(" - ")).Trim();
                    var right = node.Content.Substring(node.Content.IndexOf(" - ") + 3).Trim();

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

                    return new NameExp(node, left, right);
                }
                else
                {
                    return new NameExp(node, node.Content.Trim());
                }
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParserErrorLevel.High,
                ParseErrorType.Error));
            return default;
        }
    }
}
