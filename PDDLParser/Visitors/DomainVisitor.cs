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
    public class DomainVisitor : BaseVisitor
    {
        public static IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            if (node.Content.StartsWith("domain"))
            {
                var name = PurgeEscapeChars(node.Content).Remove(0, "domain".Length).Trim();
                return new DomainNameDecl(node, parent, name);
            }
            else if (node.Content.StartsWith(":requirements"))
            {
                var str = PurgeEscapeChars(node.Content).Remove(0, ":requirements".Length).Trim();
                var newReq = new RequirementsDecl(node, parent, new List<NameExp>());
                newReq.Requirements = LooseParseString(node, newReq, ":requirements", str, listener);
                return newReq;
            }
            else if (node.Content.StartsWith(":extends"))
            {
                var str = PurgeEscapeChars(node.Content).Remove(0, ":extends".Length).Trim();
                var newExt = new ExtendsDecl(node, parent, new List<NameExp>());
                newExt.Extends = LooseParseString(node, newExt, ":extends", str, listener);
                return newExt;
            }
            else if (node.Content.StartsWith(":types"))
            {
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeDecl>());
                var str = node.Content.Replace(":types", "");
                foreach (var typeDec in str.Split(ASTTokens.BreakToken))
                {
                    if (typeDec != "")
                    {
                        var left = typeDec.Substring(0, typeDec.IndexOf(ASTTokens.TypeToken));
                        var right = typeDec.Substring(typeDec.IndexOf(ASTTokens.TypeToken) + 3);
                        var newTypeDecl = new TypeDecl(node, newTypesDecl, null, new List<TypeNameDecl>());
                        newTypeDecl.TypeName = new TypeNameDecl(node, newTypeDecl, right);
                        foreach (var subType in left.Split(' '))
                            if (subType != "")
                                newTypeDecl.SubTypes.Add(new TypeNameDecl(node, newTypeDecl, subType));
                        newTypesDecl.Types.Add(newTypeDecl);
                    }
                }
                return newTypesDecl;
            }
            else if (node.Content.StartsWith(":constants"))
            {
                var newCons = new ConstantsDecl(node, parent, new List<NameExp>());
                newCons.Constants = LooseParseString(node, newCons, ":constants", node.Content.Replace(":constants", "").Trim(), listener);
                return newCons;
            }
            else if (node.Content.StartsWith(":predicates"))
            {
                var newPred = new PredicatesDecl(node, parent, new List<PredicateExp>());
                newPred.Predicates = ParseAsPredicateList(node, newPred, listener);
                return newPred;
            }
            else if (node.Content.StartsWith(":timeless"))
            {
                var newTime = new TimelessDecl(node, parent, new List<PredicateExp>());
                newTime.Items = ParseAsPredicateList(node, newTime, listener);
                return newTime;
            }
            else if (node.Content.StartsWith(":action"))
            {
                var actionName = node.Content.Replace(":action", "").Trim().Split(' ')[0].Trim();

                CheckIfContentIncludes(node, ":action", ":parameters", listener);
                CheckIfContentIncludes(node, ":action", ":precondition", listener);
                CheckIfContentIncludes(node, ":action", ":effect", listener);
                DoesNodeHaveSpecificChildCount(node, ":action", 3, listener);

                var newActionDecl = new ActionDecl(node, parent, actionName, new List<NameExp>(), null, null);

                // Parameters
                newActionDecl.Parameters = LooseParseString(node.Children[0], newActionDecl, ":action", node.Children[0].Content.Replace(actionName, "").Trim(), listener);

                // Preconditions
                newActionDecl.Preconditions = ExpVisitor.Visit(node.Children[1], newActionDecl, listener);

                // Effects
                newActionDecl.Effects = ExpVisitor.Visit(node.Children[2], newActionDecl, listener);

                return newActionDecl;
            }
            else if (node.Content.StartsWith(":axiom"))
            {
                CheckIfContentIncludes(node, ":axiom", ":vars", listener);
                CheckIfContentIncludes(node, ":axiom", ":context", listener);
                CheckIfContentIncludes(node, ":axiom", ":implies", listener);
                DoesNodeHaveSpecificChildCount(node, ":action", 3, listener);

                var newAxiomDecl = new AxiomDecl(node, parent, new List<NameExp>(), null, null);

                // Vars
                newAxiomDecl.Vars = LooseParseString(node.Children[0], newAxiomDecl, ":axiom", node.Children[0].Content.Trim(), listener);

                // Context
                newAxiomDecl.Context = ExpVisitor.Visit(node.Children[1], newAxiomDecl, listener);

                // Implies
                newAxiomDecl.Implies = ExpVisitor.Visit(node.Children[2], newAxiomDecl, listener);

                return newAxiomDecl;
            }

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }
    }
}
