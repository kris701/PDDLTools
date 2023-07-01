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
        public static IExp Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("and"))
            {
                DoesNodeHaveMoreThanNChildren(node, "and", 0, listener);
                DoesContentContainAnyStrayCharacters(node, "and", listener);

                List<IExp> children = new List<IExp>();
                foreach(var child in node.Children)
                    children.Add(Visit(child, listener));

                return new AndExp(node, children);
            } 
            else if (node.Content.StartsWith("or"))
            {
                DoesNodeHaveSpecificChildCount(node, "or", 2, listener);
                DoesContentContainAnyStrayCharacters(node, "or", listener);

                return new OrExp(node, Visit(node.Children[0], listener), Visit(node.Children[1], listener));
            }
            else if (node.Content.StartsWith("not"))
            {
                DoesNodeHaveSpecificChildCount(node, "not", 1, listener);
                DoesContentContainAnyStrayCharacters(node, "not", listener);

                return new NotExp(node, Visit(node.Children[0], listener));
            }
            else if (node.Content.Contains(" "))
            {
                DoesNodeHaveSpecificChildCount(node, "predicate", 0, listener);

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
                        node.Character));
                }
                if (right == "")
                {
                    listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but a type was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Character));
                }

                return new NameExp(node, left.Replace("?", ""), new TypeNameDecl(node, right));
            } 
            else
            {
                DoesNodeHaveSpecificChildCount(node, "name", 0, listener);

                return new NameExp(node, node.Content.Replace("?", "").Trim());
            }
        }
    }
}
