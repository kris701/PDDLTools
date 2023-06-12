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
    public class ExpVisitor
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
                        ParseErrorType.Warning));
                }
                return new AndExp(node, children);
            } 
            else if (node.Content.StartsWith("not"))
            {
                if (node.Children.Count == 0)
                {
                    listener.AddError(new ParseError(
                        $"'not' node does not have any children!",
                        ParserErrorLevel.High,
                        ParseErrorType.Warning,
                        node.Line,
                        node.Character));
                }
                return new NotExp(node, Visit(node.Children[0], listener));
            } 
            else
            {
                if (node.Content.Contains(" - "))
                {
                    var left = node.Content.Substring(0, node.Content.IndexOf(" - ")).Trim();
                    var right = node.Content.Substring(node.Content.IndexOf(" - ") + 3).Trim();

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
