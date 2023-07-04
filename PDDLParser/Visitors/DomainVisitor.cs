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
            IDecl returnNode = null;
            if (TryVisitDomainNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitRequirementsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitExtendsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitTypesNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitConstantsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitPredicatesNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitTimelessNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitActionNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitAxiomNode(node, parent, listener, out returnNode))
                return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }

        public static bool TryVisitDomainNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "domain"))
            {
                var name = PurgeEscapeChars(node.InnerContent).Remove(0, "domain".Length).Trim();
                decl = new DomainNameDecl(node, parent, name);

                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitRequirementsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":requirements"))
            {
                var str = PurgeEscapeChars(node.InnerContent).Remove(0, ":requirements".Length).Trim();
                var newReq = new RequirementsDecl(node, parent, new List<NameExp>());
                newReq.Requirements = LooseParseString(node, newReq, ":requirements", str, listener);

                decl = newReq;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitExtendsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":extends"))
            {
                var str = PurgeEscapeChars(node.InnerContent).Remove(0, ":extends".Length).Trim();
                var newExt = new ExtendsDecl(node, parent, new List<NameExp>());
                newExt.Extends = LooseParseString(node, newExt, ":extends", str, listener);

                decl = newExt;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitTypesNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":types"))
            {
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeDecl>());
                var str = node.InnerContent.Replace(":types", "");
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

                decl = newTypesDecl;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitConstantsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":constants"))
            {
                var newCons = new ConstantsDecl(node, parent, new List<NameExp>());
                newCons.Constants = LooseParseString(node, newCons, ":constants", node.InnerContent.Replace(":constants", "").Trim(), listener);

                decl = newCons;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitPredicatesNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":predicates"))
            {
                var newPred = new PredicatesDecl(node, parent, new List<PredicateExp>());
                newPred.Predicates = ParseAsPredicateList(node, newPred, listener);

                decl = newPred;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitTimelessNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":timeless"))
            {
                var newTime = new TimelessDecl(node, parent, new List<PredicateExp>());
                newTime.Items = ParseAsPredicateList(node, newTime, listener);

                decl = newTime;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitActionNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":action"))
            {
                var actionName = node.InnerContent.Replace(":action", "").Trim().Split(' ')[0].Trim();

                CheckIfContentIncludes(node, ":action", ":parameters", listener);
                CheckIfContentIncludes(node, ":action", ":precondition", listener);
                CheckIfContentIncludes(node, ":action", ":effect", listener);
                DoesNodeHaveSpecificChildCount(node, ":action", 3, listener);

                var newActionDecl = new ActionDecl(node, parent, actionName, new List<NameExp>(), null, null);

                // Parameters
                newActionDecl.Parameters = LooseParseString(node.Children[0], newActionDecl, ":action", node.Children[0].InnerContent.Replace(actionName, "").Trim(), listener);

                // Preconditions
                newActionDecl.Preconditions = ExpVisitor.Visit(node.Children[1], newActionDecl, listener);

                // Effects
                newActionDecl.Effects = ExpVisitor.Visit(node.Children[2], newActionDecl, listener);

                decl = newActionDecl;
                return true;
            }
            decl = null;
            return false;
        }

        public static bool TryVisitAxiomNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":axiom"))
            {
                CheckIfContentIncludes(node, ":axiom", ":vars", listener);
                CheckIfContentIncludes(node, ":axiom", ":context", listener);
                CheckIfContentIncludes(node, ":axiom", ":implies", listener);
                DoesNodeHaveSpecificChildCount(node, ":action", 3, listener);

                var newAxiomDecl = new AxiomDecl(node, parent, new List<NameExp>(), null, null);

                // Vars
                newAxiomDecl.Vars = LooseParseString(node.Children[0], newAxiomDecl, ":axiom", node.Children[0].InnerContent.Trim(), listener);

                // Context
                newAxiomDecl.Context = ExpVisitor.Visit(node.Children[1], newAxiomDecl, listener);

                // Implies
                newAxiomDecl.Implies = ExpVisitor.Visit(node.Children[2], newAxiomDecl, listener);

                decl = newAxiomDecl;
                return true;
            }
            decl = null;
            return false;
        }
    }
}
