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
    public class DomainVisitor : BaseVisitor
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
            else if (node.Content.StartsWith(":extends"))
            {
                var str = PurgeEscapeChars(node.Content).Remove(0, ":extends".Length).Trim();
                var reqs = str.Split(' ');
                List<string> extends = new List<string>();
                foreach (var req in reqs)
                    if (req != "")
                        extends.Add(req);
                return new ExtendsDecl(node, extends);
            }
            else if (node.Content.StartsWith(":types"))
            {
                List<TypeDecl> types = new List<TypeDecl>();
                foreach (var typeDec in node.Content.Split(ASTTokens.BreakToken))
                {
                    var removedType = typeDec.Replace(":types", "").Trim();
                    if (removedType != "")
                    {
                        var left = removedType.Substring(0, removedType.IndexOf(ASTTokens.TypeToken));
                        var right = removedType.Substring(removedType.IndexOf(ASTTokens.TypeToken) + 3);
                        types.Add(new TypeDecl(node, right, left.Split(' ').ToList()));
                    }
                }
                return new TypesDecl(node, types);
            }
            else if (node.Content.StartsWith(":constants"))
            {
                List<NameExp> constants = new List<NameExp>();
                foreach (var typeDec in node.Content.Split(ASTTokens.BreakToken))
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
                {
                    var exp = ExpVisitor.Visit(predicate, listener);
                    if (exp is PredicateExp pred)
                        predicates.Add(pred);
                    if (exp is NameExp cpred)
                        predicates.Add(new PredicateExp(new ASTNode(cpred.Character, cpred.Line, ""), cpred.Name, new List<NameExp>()));
                }
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

                if (!node.Content.Contains(":parameters"))
                    listener.AddError(new ParseError(
                        $"Action is malformed! missing ':parameters'",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (!node.Content.Contains(":precondition"))
                    listener.AddError(new ParseError(
                        $"Action is malformed! missing ':precondition'",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (!node.Content.Contains(":effect"))
                    listener.AddError(new ParseError(
                        $"Action is malformed! missing ':effect'",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (node.Children.Count != 3)
                    listener.AddError(new ParseError(
                        $"Action has an unexpected number of children! Expected 3, got {node.Children.Count}",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));

                // Parameters
                var parameters = LooseParseString(node, ":action", node.Children[0].Content.Replace(actionName, "").Trim(), listener);

                // Preconditions
                IExp precondition = ExpVisitor.Visit(node.Children[1], listener);
                DecorateExpressions(precondition, parameters, listener);

                // Effects
                IExp effects = ExpVisitor.Visit(node.Children[2], listener);
                DecorateExpressions(effects, parameters, listener);

                return new ActionDecl(
                    node,
                    actionName,
                    parameters,
                    precondition,
                    effects);
            }
            else if (node.Content.StartsWith(":axiom"))
            {

                if (!node.Content.Contains(":vars"))
                    listener.AddError(new ParseError(
                        $"Axiom is malformed! missing ':vars'",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (!node.Content.Contains(":context"))
                    listener.AddError(new ParseError(
                        $"Axiom is malformed! missing ':context'",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (!node.Content.Contains(":implies"))
                    listener.AddError(new ParseError(
                        $"Axiom is malformed! missing ':implies'",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));
                if (node.Children.Count != 3)
                    listener.AddError(new ParseError(
                        $"Axiom has an unexpected number of children! Expected 3, got {node.Children.Count}",
                        ParseErrorType.Error,
                        node.Line,
                        node.Character));

                // Vars
                var vars = LooseParseString(node, ":axiom", node.Children[0].Content.Trim(), listener);

                // Context
                IExp context = ExpVisitor.Visit(node.Children[1], listener);
                DecorateExpressions(context, vars, listener);

                // Implies
                IExp implies = ExpVisitor.Visit(node.Children[2], listener);
                DecorateExpressions(implies, vars, listener);

                return new AxiomDecl(
                    node,
                    vars,
                    context,
                    implies);
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParseErrorType.Error));
            return default;
        }

        private static void DecorateExpressions(IExp node, List<NameExp> parameters, IErrorListener listener)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    DecorateExpressions(child, parameters, listener);
            }
            else if (node is OrExp or)
            {
                DecorateExpressions(or.Option1, parameters, listener);
                DecorateExpressions(or.Option2, parameters, listener);
            }
            else if (node is NotExp not)
            {
                DecorateExpressions(not.Child, parameters, listener);
            }
            else if (node is PredicateExp pred)
            {
                foreach(var arg in pred.Arguments)
                {
                    if (!parameters.Any(x => x.Name == arg.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Could not match name of parameters and precondition!",
                            ParseErrorType.Error,
                            arg.Line,
                            arg.Character));
                    }
                    else
                    {
                        var item = parameters.Single(x => x.Name == arg.Name);
                        arg.Type = item.Type;
                    }
                }
            }
        }

        private static string PurgeEscapeChars(string str) => str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
    }
}
