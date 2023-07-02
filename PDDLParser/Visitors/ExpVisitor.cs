using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLParser.Models.Domain;

namespace PDDLParser.Visitors
{
    public class ExpVisitor : BaseVisitor
    {
        public static IExp Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            if (node.Content.StartsWith("and"))
            {
                DoesNodeHaveMoreThanNChildren(node, "and", 0, listener);
                DoesContentContainAnyStrayCharacters(node, "and", listener);

                var newAndExp = new AndExp(node, parent, new List<IExp>());
                foreach(var child in node.Children)
                    newAndExp.Children.Add(Visit(child, newAndExp, listener));

                return newAndExp;
            } 
            else if (node.Content.StartsWith("or"))
            {
                DoesNodeHaveSpecificChildCount(node, "or", 2, listener);
                DoesContentContainAnyStrayCharacters(node, "or", listener);

                var newOrExp = new OrExp(node, parent, null, null);
                newOrExp.Option1 = Visit(node.Children[0], newOrExp, listener);
                newOrExp.Option2 = Visit(node.Children[1], newOrExp, listener);
                return newOrExp;
            }
            else if (node.Content.StartsWith("not"))
            {
                DoesNodeHaveSpecificChildCount(node, "not", 1, listener);
                DoesContentContainAnyStrayCharacters(node, "not", listener);

                var newNotExp = new NotExp(node, parent, null);
                newNotExp.Child = Visit(node.Children[0], newNotExp, listener);
                return newNotExp;
            }
            else if (node.Content.Contains(" "))
            {
                DoesNodeHaveSpecificChildCount(node, "predicate", 0, listener);

                var predicateName = node.Content.Split(' ')[0];
                var newPredicateExp = new PredicateExp(node, parent, predicateName, new List<NameExp>());

                var paramSplit = node.Content.Split(' ');
                int offset = 0;
                foreach (var param in paramSplit)
                {
                    if (param != "" && param != predicateName)
                    {
                        newPredicateExp.Arguments.Add(Visit(new ASTNode(node.Start + offset, node.End, param), newPredicateExp, listener) as NameExp);
                    }
                    offset += param.Length;
                }
                return newPredicateExp;
            } 
            else if (node.Content.Contains(ASTTokens.TypeToken))
            {
                DoesNodeHaveSpecificChildCount(node, "name", 0, listener);

                var left = node.Content.Substring(0, node.Content.IndexOf(ASTTokens.TypeToken)).Trim();
                var right = node.Content.Substring(node.Content.IndexOf(ASTTokens.TypeToken) + 3).Trim();

                if (left == "")
                {
                    listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but an object name was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
                }
                if (right == "")
                {
                    listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but a type was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
                }

                var newNameExp = new NameExp(node, parent, left.Replace("?",""));
                newNameExp.Type = new TypeNameDecl(
                    new ASTNode(
                        node.Start + left.Length + 3, 
                        node.Start + left.Length + 3 + right.Length, 
                        right), 
                    newNameExp, 
                    right);
                return newNameExp;
            } 
            else
            {
                DoesNodeHaveSpecificChildCount(node, "name", 0, listener);

                var newNameExp = new NameExp(node, parent, node.Content.Replace("?", ""));
                return newNameExp;
            }
        }
    }
}
