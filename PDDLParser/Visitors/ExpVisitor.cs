﻿using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLParser.Models.Domain;
using System.Xml.Linq;

namespace PDDLParser.Visitors
{
    public class ExpVisitor : BaseVisitor
    {
        public static IExp Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IExp returnNode = null;
            if (TryVisitAndNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitOrNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitNotNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitPredicateNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitNameNode(node, parent, listener, out returnNode))
                return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }

        public static bool TryVisitAndNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (IsOfValidNodeType(node.InnerContent, "and"))
            {
                DoesNodeHaveMoreThanNChildren(node, "and", 0, listener);
                DoesContentContainAnyStrayCharacters(node, "and", listener);

                var newAndExp = new AndExp(node, parent, new List<IExp>());
                foreach (var child in node.Children)
                    newAndExp.Children.Add(Visit(child, newAndExp, listener));

                exp = newAndExp;
                return true;
            }
            exp = null;
            return false;
        }

        public static bool TryVisitOrNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (IsOfValidNodeType(node.InnerContent, "or"))
            {
                DoesNodeHaveSpecificChildCount(node, "or", 2, listener);
                DoesContentContainAnyStrayCharacters(node, "or", listener);

                var newOrExp = new OrExp(node, parent, null, null);
                newOrExp.Option1 = Visit(node.Children[0], newOrExp, listener);
                newOrExp.Option2 = Visit(node.Children[1], newOrExp, listener);
                exp = newOrExp;
                return true;
            }
            exp = null;
            return false;
        }

        public static bool TryVisitNotNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (IsOfValidNodeType(node.InnerContent, "not"))
            {
                DoesNodeHaveSpecificChildCount(node, "not", 1, listener);
                DoesContentContainAnyStrayCharacters(node, "not", listener);

                var newNotExp = new NotExp(node, parent, null);
                newNotExp.Child = Visit(node.Children[0], newNotExp, listener);
                exp = newNotExp;
                return true;
            }
            exp = null;
            return false;
        }

        public static bool TryVisitPredicateNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (node.OuterContent.Contains('(') && node.OuterContent.Contains(')') && node.InnerContent != "")
            {
                DoesNodeHaveSpecificChildCount(node, "predicate", 0, listener);

                var predicateName = node.InnerContent.Split(' ')[0];
                var newPredicateExp = new PredicateExp(node, parent, predicateName, new List<NameExp>());

                var paramSplit = node.InnerContent.Split(' ');
                int offset = 0;
                foreach (var param in paramSplit)
                {
                    if (param != "" && param != predicateName)
                    {
                        newPredicateExp.Arguments.Add(Visit(new ASTNode(node.Start + offset, node.End, param, param), newPredicateExp, listener) as NameExp);
                    }
                    offset += param.Length;
                }
                exp = newPredicateExp;
                return true;
            } 
            exp = null;
            return false;
        }

        public static bool TryVisitNameNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (node.InnerContent.Contains(ASTTokens.TypeToken))
            {
                DoesNodeHaveSpecificChildCount(node, "name", 0, listener);

                var left = node.InnerContent.Substring(0, node.InnerContent.IndexOf(ASTTokens.TypeToken)).Trim();
                var right = node.InnerContent.Substring(node.InnerContent.IndexOf(ASTTokens.TypeToken) + 3).Trim();

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

                var newNameExp = new NameExp(node, parent, left.Replace("?", ""));
                newNameExp.Type = new TypeNameDecl(
                    new ASTNode(
                        node.Start + left.Length + 3,
                        node.Start + left.Length + 3 + right.Length,
                        right,
                        right),
                    newNameExp,
                    right);
                exp =  newNameExp;
                return true;
            }
            else
            {
                DoesNodeHaveSpecificChildCount(node, "name", 0, listener);

                var newNameExp = new NameExp(node, parent, node.InnerContent.Replace("?", ""));
                exp = newNameExp;
                return true;
            }
        }
    }
}
