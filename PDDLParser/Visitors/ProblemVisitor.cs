using PDDLParser.AST;
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
    public static class ProblemVisitor
    {
        public static IDecl Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("problem"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, "problem".Length).Trim();
                return new ProblemNameDecl(node, name);
            } 
            else if (node.Content.StartsWith(":domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, ":domain".Length).Trim();
                return new DomainNameRefDecl(node, name);
            } 
            else if (node.Content.StartsWith(":objects"))
            {
                List<NameExp> objs = new List<NameExp>();
                foreach (var objDecl in node.Content.Split(ASTTokens.BreakToken))
                {
                    var removedType = objDecl.Replace(":objects", "").Trim();
                    if (removedType != "")
                    {
                        if (removedType.Contains(ASTTokens.TypeToken))
                        {
                            var left = removedType.Substring(0, removedType.IndexOf(ASTTokens.TypeToken));
                            var right = removedType.Substring(removedType.IndexOf(ASTTokens.TypeToken) + 3);

                            foreach (var obj in left.Split(' '))
                                objs.Add(new NameExp(node, obj, right));
                        }
                        else
                        {
                            foreach (var obj in removedType.Split(' '))
                                objs.Add(new NameExp(node, obj));
                        }
                    }
                }
                return new ObjectsDecl(node, objs);
            }
            else if (node.Content.StartsWith(":init"))
            {
                List<PredicateExp> inits = new List<PredicateExp>();
                foreach(var child in node.Children)
                    inits.Add(ProblemExpVisitor.Visit(new ASTNode(child.Character, child.Line, child.Content), listener) as PredicateExp);
                return new InitDecl(node, inits);
            }
            else if (node.Content.StartsWith(":goal"))
            {
                return new GoalDecl(node, ProblemExpVisitor.Visit(node.Children[0], listener));
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParserErrorLevel.High,
                ParseErrorType.Error));
            return default;
        }

        private static string PurgeEscapeChars(string str) => str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
    }
}
