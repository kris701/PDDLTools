using PDDLParser.AST;
using PDDLParser.Domain;
using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Visitors
{
    public static class DeclVisitor
    {
        public static IDecl Visit(ASTNode node)
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
                    if (removedType.Contains("-"))
                    {
                        string[] split = removedType.Split('-');
                        string left = split[0];
                        string right = split[1];

                        var subTypes = new List<string>();
                        foreach (var subType in left.Split(' '))
                            if (subType != " " && subType != "")
                                subTypes.Add(subType);

                        types.Add(new TypeDecl(right, subTypes));
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
                    if (removedType.Contains("-"))
                    {
                        string[] split = removedType.Split('-');
                        string left = split[0];
                        string right = split[1];

                        constants.Add(new NameExp(left, right));
                    }
                    else
                    {
                        constants.Add(new NameExp(removedType));
                    }
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
                    {
                        if (arg != "")
                        {
                            if (arg.Contains("-"))
                            {
                                var argSplit = arg.Split('-');
                                var left = argSplit[0];
                                var right = argSplit[1];
                                argList.Add(new NameExp(left, right));
                            }
                            else
                            {
                                argList.Add(new NameExp(arg));
                            }
                        }
                    }
                    predicates.Add(new PredicateDecl(predicateName, argList));
                }
                return new PredicatesDecl(predicates);
            }
            else if (node.Content.StartsWith(":action"))
            {
                var actionName = node.Content.Replace(":action", "").Trim().Split(' ')[0].Trim();

                // Parameters
                List<NameExp> parameters = new List<NameExp>();
                var split = node.Children[0].Content.Split('?');
                foreach(var item in split)
                    if (item != "")
                        parameters.Add(ExpVisitor.Visit(new ASTNode(item)) as NameExp);

                // Preconditions
                IExp precondition = ExpVisitor.Visit(node.Children[1]);

                // Effects
                IExp effects = ExpVisitor.Visit(node.Children[2]);

                return new ActionDecl(
                    actionName,
                    parameters,
                    precondition,
                    effects);
            }

            throw new ParseException(
                $"Could not parse content of AST node: {node.Content}",
                ParserErrorLevel.High,
                ParseErrorCategory.Error,
                -1
                );
        }

        private static string PurgeEscapeChars(string str) => str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
    }
}
