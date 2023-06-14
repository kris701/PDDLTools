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
            else
            {
                var name = node.Content.Split(' ')[0];
                var args = new List<NameExp>();
                foreach (var arg in node.Content.Split(' '))
                    if (arg != name && arg != "")
                        args.Add(new NameExp(node, arg));
                return new PredicateExp(node, name, args);

            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParserErrorLevel.High,
                ParseErrorType.Error));
            return default;
        }
    }
}
