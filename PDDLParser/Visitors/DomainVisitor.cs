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
    public class DomainVisitor : BaseVisitor, IVisitor<ASTNode, INode, IDecl>
    {
        public IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IDecl returnNode = null;
            if (TryVisitDomainDeclNode(node, parent, listener, out returnNode))
                return returnNode;
            if (TryVisitDomainNameNode(node, parent, listener, out returnNode))
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
                ParseErrorLevel.Parsing,
                ParserErrorCode.UnknownNode));
            return default;
        }

        public bool TryVisitDomainDeclNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "define"))
            {
                if (DoesNotContainStrayCharacters(node, "define", listener))
                {
                    var returnDomain = new DomainDecl(node);
                    foreach (var child in node.Children)
                    {
                        var visited = Visit(child, returnDomain, listener);
                        if (visited is DomainNameDecl domainName)
                            returnDomain.Name = domainName;
                        else if (visited is RequirementsDecl requirements)
                            returnDomain.Requirements = requirements;
                        else if (visited is ExtendsDecl extends)
                            returnDomain.Extends = extends;
                        else if (visited is TypesDecl types)
                            returnDomain.Types = types;
                        else if (visited is ConstantsDecl constants)
                            returnDomain.Constants = constants;
                        else if (visited is TimelessDecl timeless)
                            returnDomain.Timeless = timeless;
                        else if (visited is PredicatesDecl predicates)
                            returnDomain.Predicates = predicates;
                        else if (visited is ActionDecl act)
                        {
                            if (returnDomain.Actions == null)
                                returnDomain.Actions = new List<ActionDecl>();
                            returnDomain.Actions.Add(act);
                        }
                        else if (visited is AxiomDecl axi)
                        {
                            if (returnDomain.Axioms == null)
                                returnDomain.Axioms = new List<AxiomDecl>();
                            returnDomain.Axioms.Add(axi);
                        }
                    }
                    decl = returnDomain;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitDomainNameNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "domain"))
            {
                if (DoesContentContainNLooseChildren(node, "domain", 1, listener))
                {
                    var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "domain");
                    decl = new DomainNameDecl(node, parent, name);
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitRequirementsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":requirements"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":requirements");
                var newReq = new RequirementsDecl(node, parent, new List<NameExp>());
                newReq.Requirements = LooseParseString(node, newReq, ":requirements", str, listener);

                decl = newReq;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitExtendsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":extends"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":extends");
                var newExt = new ExtendsDecl(node, parent, new List<NameExp>());
                newExt.Extends = LooseParseString(node, newExt, ":extends", str, listener);

                decl = newExt;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitTypesNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":types"))
            {
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeDecl>());
                var str = node.InnerContent.Remove(node.InnerContent.IndexOf(":types"), ":types".Length).Trim();
                foreach (var typeDec in str.Split(ASTTokens.BreakToken))
                {
                    if (typeDec != "")
                    {
                        var left = typeDec.Substring(0, typeDec.IndexOf(ASTTokens.TypeToken));
                        var right = typeDec.Substring(typeDec.IndexOf(ASTTokens.TypeToken) + 3);
                        var newTypeDecl = new TypeDecl(node, newTypesDecl, null, new List<TypeNameDecl>());
                        newTypeDecl.TypeName = new TypeNameDecl(node, newTypeDecl, right.Trim());
                        foreach (var subType in left.Split(' '))
                            if (subType != "")
                                newTypeDecl.SubTypes.Add(new TypeNameDecl(node, newTypeDecl, subType.Trim()));
                        newTypesDecl.Types.Add(newTypeDecl);
                    }
                }

                decl = newTypesDecl;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitConstantsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":constants"))
            {
                var newCons = new ConstantsDecl(node, parent, new List<NameExp>());
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":constants");
                newCons.Constants = LooseParseString(node, newCons, ":constants", str, listener);

                decl = newCons;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitPredicatesNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
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

        public bool TryVisitTimelessNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":timeless"))
            {
                var newTime = new TimelessDecl(node, parent, new List<NameExp>());
                foreach (var child in node.Children)
                    child.OuterContent = child.InnerContent;
                newTime.Items = ParseAsNameList(node, newTime, listener);

                decl = newTime;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitActionNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":action"))
            {
                if (DoesContentContainTarget(node, ":action", ":parameters", listener) &&
                    DoesContentContainTarget(node, ":action", ":precondition", listener) &&
                    DoesContentContainTarget(node, ":action", ":effect", listener) &&
                    DoesNodeHaveSpecificChildCount(node, ":action", 3, listener) &&
                    DoesContentContainNLooseChildren(node, ":action", 4, listener))
                {

                    var nameFindStr = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, ":action"));
                    var actionName = nameFindStr.Split(' ')[0].Trim();

                    var newActionDecl = new ActionDecl(node, parent, actionName, new List<NameExp>(), null, null);
                    var visitor = new ExpVisitor();

                    // Parameters
                    newActionDecl.Parameters = LooseParseString(node.Children[0], newActionDecl, ":action", node.Children[0].InnerContent, listener);

                    // Preconditions
                    newActionDecl.Preconditions = visitor.Visit(node.Children[1], newActionDecl, listener);

                    // Effects
                    newActionDecl.Effects = visitor.Visit(node.Children[2], newActionDecl, listener);

                    decl = newActionDecl;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitAxiomNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":axiom"))
            {
                if (DoesContentContainTarget(node, ":axiom", ":vars", listener) &&
                    DoesContentContainTarget(node, ":axiom", ":context", listener) &&
                    DoesContentContainTarget(node, ":axiom", ":implies", listener) &&
                    DoesNodeHaveSpecificChildCount(node, ":axiom", 3, listener) &&
                    DoesContentContainNLooseChildren(node, ":axiom", 3, listener))
                {

                    var newAxiomDecl = new AxiomDecl(node, parent, new List<NameExp>(), null, null);
                    var visitor = new ExpVisitor();

                    // Vars
                    newAxiomDecl.Vars = LooseParseString(node.Children[0], newAxiomDecl, ":axiom", node.Children[0].InnerContent.Trim(), listener);

                    // Context
                    newAxiomDecl.Context = visitor.Visit(node.Children[1], newAxiomDecl, listener);

                    // Implies
                    newAxiomDecl.Implies = visitor.Visit(node.Children[2], newAxiomDecl, listener);

                    decl = newAxiomDecl;
                    return true;
                }
            }
            decl = null;
            return false;
        }
    }
}
