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
                    node.Line,
                    node.Character));
        }

        internal static void DoesNodeHaveSpecificChildCount(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (targetChildren == 0)
            {
                if (node.Children.Count != 0)
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must not contain any children!",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
            }
            else
            {
                if (node.Children.Count != targetChildren)
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must have exactly {targetChildren} children, but it has '{node.Children.Count}'!",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
            }
        }

        internal static void DoesNodeHaveMoreThanNChildren(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (node.Children.Count <= targetChildren)
                listener.AddError(new ParseError(
                    $"'{nodeName}' must have more than {targetChildren} children, but it has '{node.Children.Count}'!",
                    ParseErrorType.Error,
                    node.Line,
                    node.Character));
        }

        internal static List<NameExp> LooseParseString(ASTNode node, string nodeType, string content, IErrorListener listener)
        {
            List<NameExp> objs = new List<NameExp>();
            foreach (var param in content.Split(' '))
            {
                if (param != "" && param != nodeType)
                {
                    var parsed = ExpVisitor.Visit(new ASTNode(
                        node.Character,
                        node.Line,
                        param), listener);
                    if (parsed is NameExp nExp)
                        objs.Add(nExp);
                    else
                    {
                        listener.AddError(new ParseError(
                            $"Unexpected node type while parsing '{nodeType}'!",
                            ParseErrorType.Error,
                            parsed.Line,
                            parsed.Character));
                    }
                }
            }
            return objs;
        }
    }
}
