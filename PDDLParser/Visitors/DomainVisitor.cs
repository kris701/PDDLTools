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
using PDDLParser.Helpers;

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
                newReq.Requirements = LooseParseString<NameExp>(node, newReq, ":requirements", str, listener);

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
                newExt.Extends = LooseParseString<NameExp>(node, newExt, ":extends", str, listener);

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
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeExp>());
                var newTypes = new List<TypeExp>();
                var str = node.InnerContent.Remove(node.InnerContent.IndexOf(":types"), ":types".Length).Trim();
                str = ReduceToSingleSpace(str);

                int indexOffset = 0;
                foreach (var line in str.Split('\n'))
                {
                    if (line != "")
                    {
                        int thisOffset = 0;
                        var editLine = PurgeEscapeChars(line);
                        HashSet<string> subTypes = new HashSet<string>() { "" };
                        while (editLine.Contains(ASTTokens.TypeToken))
                        {
                            var innerContent = editLine.Substring(editLine.LastIndexOf(ASTTokens.TypeToken) + ASTTokens.TypeToken.Length);

                            TypeExp newCurrentType = null;
                            foreach (var param in innerContent.Split(' ').Reverse())
                            {
                                if (param != "")
                                {
                                    var newType = new TypeExp(new ASTNode(), newTypesDecl, param, subTypes);
                                    newTypes.Insert(indexOffset, newType);
                                    thisOffset++;
                                    newCurrentType = newType;
                                }
                            }
                            subTypes.Add(newCurrentType.Name);

                            editLine = editLine.Substring(0, editLine.LastIndexOf(ASTTokens.TypeToken));
                        }

                        foreach (var param in editLine.Split(' ').Reverse())
                        {
                            if (param != "")
                            {
                                var newType = new TypeExp(new ASTNode(), newTypesDecl, param, subTypes);
                                newTypes.Insert(indexOffset, newType);
                                thisOffset++;
                            }
                        }
                        indexOffset += thisOffset;
                    }
                }
                newTypes.Add(new TypeExp(new ASTNode(), newTypesDecl, ""));

                foreach(var type in newTypes)
                {
                    if (!newTypesDecl.Types.Any(x => x.Name == type.Name))
                    {
                        var all = newTypes.FindAll(x => x.Name == type.Name);
                        if (all.Count > 1)
                        {
                            HashSet<string> merged = new HashSet<string>();
                            foreach (var copyType in all)
                                merged.AddRange(copyType.SuperTypes);
                            newTypesDecl.Types.Add(new TypeExp(new ASTNode(), newTypesDecl, type.Name, merged));
                        }
                        else
                            newTypesDecl.Types.Add(type);
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
                newCons.Constants = LooseParseString<NameExp>(node, newCons, ":constants", str, listener);

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
                    newActionDecl.Parameters = LooseParseString<NameExp>(node.Children[0], newActionDecl, ":action", node.Children[0].InnerContent, listener);

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
                    newAxiomDecl.Vars = LooseParseString<NameExp>(node.Children[0], newAxiomDecl, ":axiom", node.Children[0].InnerContent.Trim(), listener);

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
