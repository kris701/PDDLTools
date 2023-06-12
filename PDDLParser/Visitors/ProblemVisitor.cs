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
                return new ProblemNameDecl(name);
            } 
            else if (node.Content.StartsWith(":domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, ":domain".Length).Trim();
                return new DomainNameRefDecl(name);
            } 
            else if (node.Content.StartsWith(":objects"))
            {
                List<NameExp> objs = new List<NameExp>();
                foreach (var objDecl in node.Content.Split('\n'))
                {
                    var removedType = objDecl.Replace(":objects", "").Trim();
                    if (removedType != "")
                        objs.Add(ExpVisitor.Visit(new ASTNode(removedType), listener) as NameExp);
                }
                return new ObjectsDecl(objs);
            }
            else if (node.Content.StartsWith(":init"))
            {
                List<NameExp> inits = new List<NameExp>();
                foreach(var child in node.Children)
                    inits.Add(ExpVisitor.Visit(new ASTNode(child.Content), listener) as NameExp);
                return new InitDecl(inits);
            }
            else if (node.Content.StartsWith(":goal"))
            {
                return new GoalDecl(ExpVisitor.Visit(node.Children[0], listener));
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
