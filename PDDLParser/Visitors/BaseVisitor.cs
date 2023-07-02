using PDDLParser.AST;
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
        internal static void DoesContentContainAnyStrayCharacters(ASTNode node, string targetName, IErrorListener listener)
        {
            if (node.Content.Replace(targetName, "").Trim() != "")
                listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.Content.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
        }

        internal static void DoesNodeHaveSpecificChildCount(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (targetChildren == 0)
            {
                if (node.Children.Count != 0)
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must not contain any children!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
            }
            else
            {
                if (node.Children.Count != targetChildren)
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must have exactly {targetChildren} children, but it has '{node.Children.Count}'!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
            }
        }

        internal static void DoesNodeHaveMoreThanNChildren(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (node.Children.Count <= targetChildren)
                listener.AddError(new ParseError(
                    $"'{nodeName}' must have more than {targetChildren} children, but it has '{node.Children.Count}'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
        }

        internal static List<NameExp> LooseParseString(ASTNode node, INode parent, string nodeType, string content, IErrorListener listener)
        {
            List<NameExp> objs = new List<NameExp>();
            int offset = node.Start;
            if (node.Content.Contains(nodeType))
                offset += nodeType.Length;
            foreach (var param in content.Split(' '))
            {
                if (param != "" && param != nodeType)
                {
                    var parsed = ExpVisitor.Visit(new ASTNode(
                        offset,
                        offset + param.Length,
                        node.Line,
                        param), parent, listener);
                    if (parsed is NameExp nExp)
                        objs.Add(nExp);
                    else
                    {
                        listener.AddError(new ParseError(
                            $"Unexpected node type while parsing '{nodeType}'!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing,
                            parsed.Line,
                            parsed.Start));
                    }
                }
                offset += param.Length;
            }
            return objs;
        }

        internal static List<PredicateExp> ParseAsPredicateList(ASTNode node, INode parent, IErrorListener listener)
        {
            List<PredicateExp> predicates = new List<PredicateExp>();
            foreach (var predicate in node.Children)
            {
                var newNode = ExpVisitor.Visit(predicate, parent, listener) as PredicateExp;
                if (newNode == null)
                    listener.AddError(new ParseError(
                        $"Could not parse predicate!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
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
                var newNode = ExpVisitor.Visit(child, parent, listener) as NameExp;
                if (newNode == null)
                    listener.AddError(new ParseError(
                        $"Could not parse name!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        child.Line,
                        child.Start));
                name.Add(newNode);
            }
            return name;
        }

        internal static void CheckIfContentIncludes(ASTNode node, string nodeName, string targetName, IErrorListener listener)
        {
            if (!node.Content.Contains(targetName))
                listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! missing '{targetName}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
        }

        internal static string PurgeEscapeChars(string str) => str.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
    }
}
