﻿using PDDLParser.AST;
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
        public static IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IDecl returnNode = null;
            if (TryVisitProblemDeclNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitProblemNameNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitDomainRefNameNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitObjectsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitInitsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitGoalNode(node, parent, listener, out returnNode))
                return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing,
                ParserErrorCode.UnknownNode));
            return default;
        }

        public static bool TryVisitProblemDeclNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "define"))
            {
                if (DoesNotContainStrayCharacters(node, "define", listener))
                {
                    var returnProblem = new ProblemDecl(node);
                    foreach (var child in node.Children)
                    {
                        var visited = ProblemVisitor.Visit(child, returnProblem, listener);
                        if (visited is ProblemNameDecl name)
                            returnProblem.Name = name;
                        else if (visited is DomainNameRefDecl domainName)
                            returnProblem.DomainName = domainName;
                        else if (visited is ObjectsDecl objects)
                            returnProblem.Objects = objects;
                        else if (visited is InitDecl inits)
                            returnProblem.Init = inits;
                        else if (visited is GoalDecl goal)
                            returnProblem.Goal = goal;
                    }
                    decl = returnProblem;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public static bool TryVisitProblemNameNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "problem"))
            {
                if (DoesContentContainNLooseChildren(node, "problem", 1, listener))
                {
                    var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "problem");
                    decl = new ProblemNameDecl(node, parent, name);
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public static bool TryVisitDomainRefNameNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "domain"))
            {
                if (DoesContentContainNLooseChildren(node, "domain", 1, listener))
                {
                    var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "domain");
                    decl = new DomainNameRefDecl(node, parent, name);
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public static bool TryVisitObjectsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":objects"))
            {
                if (DoesNodeHaveSpecificChildCount(node, ":objects", 0, listener))
                {
                    var newObjs = new ObjectsDecl(node, parent, new List<NameExp>());

                    var parseStr = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":objects");
                    newObjs.Objs = LooseParseString(node, newObjs, ":objects", parseStr, listener);

                    decl = newObjs;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public static bool TryVisitInitsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":init"))
            {
                if (DoesNotContainStrayCharacters(node, ":init", listener))
                {
                    var newInit = new InitDecl(node, parent, new List<PredicateExp>());
                    newInit.Predicates = ParseAsPredicateList(node, newInit, listener);
                    decl = newInit;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public static bool TryVisitGoalNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":goal"))
            {
                if (DoesNodeHaveSpecificChildCount(node, ":goal", 1, listener) &&
                    DoesNotContainStrayCharacters(node, ":goal", listener))
                {
                    var newGoal = new GoalDecl(node, parent, null);
                    newGoal.GoalExp = ExpVisitor.Visit(node.Children[0], newGoal, listener);
                    decl = newGoal;
                    return true;
                }
            }
            decl = null;
            return false;
        }
    }
}
