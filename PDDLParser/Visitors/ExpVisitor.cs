using PDDLParser.AST;
using PDDLParser.Domain;
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
                return new AndExp(children);
            } 
            else if (node.Content.StartsWith("not"))
            {
                return new NotExp(Visit(node.Children[0], listener));
            } 
            else
            {
                if (node.Content.Contains("-"))
                {
                    var contentSplit = node.Content.Split('-');
                    return new NameExp(contentSplit[0], contentSplit[1]);
                }
                else
                {
                    return new NameExp(node.Content);
                }
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParserErrorLevel.High,
                ParseErrorType.Error,
                -1));
            return default;
        }
    }
}
