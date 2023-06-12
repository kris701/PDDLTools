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
using System.Security.Cryptography;

namespace PDDLParser.Visitors
{
    public static class DomainVisitor
    {
        public static IDecl Visit(ASTNode node, IErrorListener listener)
        {
            if (node.Content.StartsWith("domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, "domain".Length).Trim();
                return new DomainNameDecl(node, name);
            }
            else if (node.Content.StartsWith(":requirements"))
            {
                var str = PurgeEscapeChars(node.Content).Remove(0, ":requirements".Length).Trim();
                var reqs = str.Split(' ');
                List<string> requirements = new List<string>();
                foreach (var req in reqs)
                    if (req != "")
                        requirements.Add(req);
                return new RequirementsDecl(node, requirements);
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
                        types.Add(new TypeDecl(node, right, left.Split(' ').ToList()));
                    }
                }
                return new TypesDecl(node, types);
            }
            else if (node.Content.StartsWith(":constants"))
            {
                List<NameExp> constants = new List<NameExp>();
                foreach (var typeDec in node.Content.Split('\n'))
                {
                    var removedType = typeDec.Replace(":constants", "").Trim();
                    constants.Add(ExpVisitor.Visit(new ASTNode(node.Character, node.Line, removedType), listener) as NameExp);
                }
                return new ConstantsDecl(node, constants);
            }
            else if (node.Content.StartsWith(":predicates"))
            {
                List<PredicateExp> predicates = new List<PredicateExp>();
                foreach (var predicate in node.Children)
                    predicates.Add(ExpVisitor.Visit(predicate, listener) as PredicateExp);
                return new PredicatesDecl(node, predicates);
            }
            else if (node.Content.StartsWith(":timeless"))
            {
                List<NameExp> items = new List<NameExp>();
                foreach (var child in node.Children)
                    items.Add(ExpVisitor.Visit(child, listener) as NameExp);
                
                return new TimelessDecl(node, items);
            }
            else if (node.Content.StartsWith(":action"))
            {
                var actionName = node.Content.Replace(":action", "").Trim().Split(' ')[0].Trim();

                // Parameters
                List<NameExp> parameters = new List<NameExp>();
                var paramSplit = node.Children[0].Content.Split('?');
                foreach (var param in paramSplit)
                    if (param != "")
                        parameters.Add(ExpVisitor.Visit(new ASTNode(node.Character, node.Line, param), listener) as NameExp);
                foreach (var param in parameters)
                    param.Name = $"?{param.Name}";

                // Preconditions
                IExp precondition = ExpVisitor.Visit(node.Children[1], listener);

                // Effects
                IExp effects = ExpVisitor.Visit(node.Children[2], listener);

                return new ActionDecl(
                    node,
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
                        vars.Add(ExpVisitor.Visit(new ASTNode(node.Character, node.Line, param), listener) as NameExp);
                foreach (var param in vars)
                    param.Name = $"?{param.Name}";

                // Context
                IExp context = ExpVisitor.Visit(node.Children[1], listener);

                // Implies
                IExp implies = ExpVisitor.Visit(node.Children[2], listener);

                return new AxiomDecl(
                    node,
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
