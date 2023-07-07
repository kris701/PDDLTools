﻿using PDDLParser.AST;
using PDDLParser.Listener;
using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Visitors
{
    public abstract class BaseVisitor
    {
        internal static bool DoesNotContainStrayCharacters(ASTNode node, string targetName, IErrorListener listener)
        {
            if (node.InnerContent.Replace(targetName, "").Trim() != "")
            {
                listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.OuterContent.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    ParserErrorCode.StrayCharactersFound,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static bool DoesNodeHaveSpecificChildCount(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (targetChildren == 0)
            {
                if (node.Children.Count != 0)
                {
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must not contain any children!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        ParserErrorCode.NoChildrenAllowed,
                        node.Line,
                        node.Start));
                    return false;
                }
            }
            else
            {
                if (node.Children.Count != targetChildren)
                {
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must have exactly {targetChildren} children, but it has '{node.Children.Count}'!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        ParserErrorCode.NeedExactChildren,
                        node.Line,
                        node.Start));
                    return false;
                }
            }
            return true;
        }

        internal static bool DoesNodeHaveMoreThanNChildren(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (node.Children.Count <= targetChildren)
            {
                listener.AddError(new ParseError(
                    $"'{nodeName}' must have more than {targetChildren} children, but it has '{node.Children.Count}'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    ParserErrorCode.MustHaveMoreThanChildren,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static List<T> LooseParseString<T>(ASTNode node, INode parent, string nodeType, string content, IErrorListener listener)
        {
            List<T> objs = new List<T>();
            int offset = node.Start;
            if (node.InnerContent.Contains(nodeType))
                offset += node.InnerContent.IndexOf(nodeType) + nodeType.Length;
            content = PurgeEscapeChars(ReduceToSingleSpace(content));

            string currentType = "";
            foreach (var param in content.Split(' ').Reverse())
            {
                if (param != "" && param != nodeType)
                {
                    var typedParam = param;
                    if (typedParam.Contains(ASTTokens.TypeToken))
                    {
                        currentType = typedParam.Substring(typedParam.IndexOf(ASTTokens.TypeToken) + ASTTokens.TypeToken.Length);
                        if (typedParam.Substring(0, typedParam.IndexOf(ASTTokens.TypeToken)).Trim() == "")
                            continue;
                    }
                    else if (!typedParam.Contains(ASTTokens.TypeToken) && currentType != "")
                        typedParam = $"{typedParam}{ASTTokens.TypeToken}{currentType}";

                    var parsed = new ExpVisitor().Visit(new ASTNode(
                        offset,
                        offset + param.Length,
                        node.Line,
                        typedParam,
                        typedParam), parent, listener);
                    if (parsed is T nExp)
                        objs.Add(nExp);
                    else
                    {
                        listener.AddError(new ParseError(
                            $"Unexpected node type while parsing! Expected '{nodeType}' but got {nameof(T)}!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing,
                            ParserErrorCode.UnexpectedNodeType,
                            parsed.Line,
                            parsed.Start));
                    }
                }
                offset += param.Length;
            }
            objs.Reverse();
            return objs;
        }

        internal static List<PredicateExp> ParseAsPredicateList(ASTNode node, INode parent, IErrorListener listener)
        {
            List<PredicateExp> predicates = new List<PredicateExp>();
            foreach (var predicate in node.Children)
            {
                var newNode = new ExpVisitor().Visit(predicate, parent, listener) as PredicateExp;
                if (newNode == null)
                    listener.AddError(new ParseError(
                        $"Could not parse predicate!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        ParserErrorCode.CouldNotParsePredicate,
                        predicate.Line,
                        predicate.Start));
                predicates.Add(newNode);
            }
            return predicates;
        }

        internal static List<NameExp> ParseAsNameList(ASTNode node, INode parent, IErrorListener listener)
        {
            List<NameExp> name = new List<NameExp>();
            foreach (var child in node.Children)
            {
                var newNode = new ExpVisitor().Visit(child, parent, listener) as NameExp;
                if (newNode == null)
                    listener.AddError(new ParseError(
                        $"Could not parse name!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        ParserErrorCode.CouldNotParseName,
                        child.Line,
                        child.Start));
                name.Add(newNode);
            }
            return name;
        }

        internal static bool DoesContentContainTarget(ASTNode node, string nodeName, string targetName, IErrorListener listener)
        {
            if (!node.InnerContent.Contains(targetName))
            {
                listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! missing '{targetName}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    ParserErrorCode.UnexpectedNodeType,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static bool DoesContentContainNLooseChildren(ASTNode node, string nodeName, int target, IErrorListener listener)
        {
            var looseChildren = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, nodeName));
            var split = looseChildren.Split(' ');
            var actualCount = split.Length;
            if (split.Length == 1)
                if (split[0] == "")
                    actualCount--;
            if (actualCount != target)
            {
                listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! Expected {target} loose children but got {actualCount}.",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    ParserErrorCode.NeedExactLooseChildren,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static bool IsOfValidNodeType(string content, string nodeType)
        {
            if (content.StartsWith(nodeType))
            {
                if (nodeType.Length == content.Length)
                    return true;
                var nextCharacter = content[nodeType.Length];
                if (nextCharacter == ' ')
                    return true;
                if (nextCharacter == '(')
                    return true;
                if (nextCharacter == '\n')
                    return true;
            }
            return false;
        }

        internal static string RemoveNodeTypeAndEscapeChars(string content, string nodeType)
        {
            return PurgeEscapeChars(content).Remove(content.IndexOf(nodeType), nodeType.Length).Trim();
        }

        internal static string ReduceToSingleSpace(string text)
        {
            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            return text;
        }

        internal static string PurgeEscapeChars(string str) => str.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
    }
}
