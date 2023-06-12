using PDDLParser.AST;
using PDDLParser.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Visitors
{
    public class ExpVisitor
    {
        public static IExp Visit(ASTNode node)
        {
            if (node.Content.StartsWith("and"))
            {
                List<IExp> children = new List<IExp>();
                foreach(var child in node.Children)
                    children.Add(Visit(child));
                return new AndExp(children);
            } 
            else if (node.Content.StartsWith("not"))
            {
                return new NotExp(Visit(node.Children[0]));
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
        }
    }
}
