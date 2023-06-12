using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Exceptions;
using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLParser.Models.Domain;

namespace PDDLParser.Visitors
{
    public static class DomainVisitor
    {
        public static IDecl Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, "domain".Length).Trim();
                return new DomainNameDecl(name);
            }
            else if (node.Content.StartsWith(":requirements"))
            {
                var str = PurgeEscapeChars(node.Content).Remove(0, ":requirements".Length).Trim();
                var reqs = str.Split(' ');
                List<string> requirements = new List<string>();
                foreach (var req in reqs)
                    if (req != "")
                        requirements.Add(req);
                return new RequirementsDecl(requirements);
            }
            else if (node.Content.StartsWith(":types"))
            {
                List<TypeDecl> types = new List<TypeDecl>();
                foreach (var typeDec in node.Content.Split('\n'))
                {
                    var removedType = typeDec.Replace(":types", "").Trim();
                    if (removedType != "")
                    {
                        var left = removedType.Substring(0, removedType.IndexOf(" - "));
                        var right = removedType.Substring(removedType.IndexOf(" - ") + 3);
                        types.Add(new TypeDecl(right, left.Split(' ').ToList()));
                    }
                }
                return new TypesDecl(types);
            }
            else if (node.Content.StartsWith(":constants"))
            {
                List<NameExp> constants = new List<NameExp>();
                foreach (var typeDec in node.Content.Split('\n'))
                {
                    var removedType = typeDec.Replace(":constants", "").Trim();
                    constants.Add(ExpVisitor.Visit(new ASTNode(removedType), listener) as NameExp);
                }
                return new ConstantsDecl(constants);
            }
            else if (node.Content.StartsWith(":predicates"))
            {
                List<PredicateDecl> predicates = new List<PredicateDecl>();
                foreach (var predicate in node.Children)
                {
                    var predicateName = predicate.Content.Split(' ')[0];
                    var argList = new List<NameExp>();

                    var argsStr = predicate.Content.Replace(predicateName, "").Trim();
                    var args = argsStr.Split('?');
                    foreach (var arg in args)
                        if (arg != "")
                            argList.Add(ExpVisitor.Visit(new ASTNode($"?{arg}"), listener) as NameExp);
                    predicates.Add(new PredicateDecl(predicateName, argList));
                }
                return new PredicatesDecl(predicates);
            }
            else if (node.Content.StartsWith(":timeless"))
            {
                List<NameExp> items = new List<NameExp>();
                foreach (var child in node.Children)
                    items.Add(ExpVisitor.Visit(child, listener) as NameExp);
                
                return new TimelessDecl(items);
            }
            else if (node.Content.StartsWith(":action"))
            {
                var actionName = node.Content.Replace(":action", "").Trim().Split(' ')[0].Trim();

                // Parameters
                List<NameExp> parameters = new List<NameExp>();
                var paramSplit = node.Children[0].Content.Split('?');
                foreach (var param in paramSplit)
                    if (param != "")
                        parameters.Add(ExpVisitor.Visit(new ASTNode($"?{param}"), listener) as NameExp);

                // Preconditions
                IExp precondition = ExpVisitor.Visit(node.Children[1], listener);

                // Effects
                IExp effects = ExpVisitor.Visit(node.Children[2], listener);

                return new ActionDecl(
                    actionName,
                    parameters,
                    precondition,
                    effects);
            }
            else if (node.Content.StartsWith(":axiom"))
            {
                // Vars
                List<NameExp> vars = new List<NameExp>();
                var varsSplit = node.Children[0].Content.Split('?');
                foreach (var param in varsSplit)
                    if (param != "")
                        vars.Add(ExpVisitor.Visit(new ASTNode($"?{param}"), listener) as NameExp);

                // Context
                IExp context = ExpVisitor.Visit(node.Children[1], listener);

                // Implies
                IExp implies = ExpVisitor.Visit(node.Children[2], listener);

                return new AxiomDecl(
                    vars,
                    context,
                    implies);
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
