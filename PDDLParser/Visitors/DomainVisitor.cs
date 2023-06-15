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
                var requirements = LooseParseString(node, ":requirements", str, listener);
                return new RequirementsDecl(node, requirements);
            }
            else if (node.Content.StartsWith(":extends"))
            {
                var str = PurgeEscapeChars(node.Content).Remove(0, ":extends".Length).Trim();
                var extends = LooseParseString(node, ":extends", str, listener);
                return new ExtendsDecl(node, extends);
            }
            else if (node.Content.StartsWith(":types"))
            {
                List<TypeDecl> types = new List<TypeDecl>();
                var str = node.Content.Replace(":types", "");
                foreach (var typeDec in str.Split(ASTTokens.BreakToken))
                {
                    if (typeDec != "")
                    {
                        var left = typeDec.Substring(0, typeDec.IndexOf(ASTTokens.TypeToken));
                        var right = typeDec.Substring(typeDec.IndexOf(ASTTokens.TypeToken) + 3);
                        List<string> subTypes = new List<string>();
                        foreach (var subType in left.Split(' '))
                            if (subType != "")
                                subTypes.Add(subType);
                        types.Add(new TypeDecl(node, right, subTypes));
                    }
                }
                return new TypesDecl(node, types);
            }
            else if (node.Content.StartsWith(":constants"))
            {
                var constants = LooseParseString(node, ":constants", node.Content.Replace(":constants", "").Trim(), listener);
                return new ConstantsDecl(node, constants);
            }
            else if (node.Content.StartsWith(":predicates"))
            {
                List<PredicateExp> predicates = ParseAsPredicateList(node, listener);
                return new PredicatesDecl(node, predicates);
            }
            else if (node.Content.StartsWith(":timeless"))
            {
                List<PredicateExp> items = ParseAsPredicateList(node, listener);
                return new TimelessDecl(node, items);
            }
            else if (node.Content.StartsWith(":action"))
            {
                var actionName = node.Content.Replace(":action", "").Trim().Split(' ')[0].Trim();

                CheckIfContentIncludes(node, ":action", ":parameters", listener);
                CheckIfContentIncludes(node, ":action", ":precondition", listener);
                CheckIfContentIncludes(node, ":action", ":effect", listener);
                DoesNodeHaveSpecificChildCount(node, ":action", 3, listener);

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
                CheckIfContentIncludes(node, ":axiom", ":vars", listener);
                CheckIfContentIncludes(node, ":axiom", ":context", listener);
                CheckIfContentIncludes(node, ":axiom", ":implies", listener);
                DoesNodeHaveSpecificChildCount(node, ":action", 3, listener);

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
    }
}
