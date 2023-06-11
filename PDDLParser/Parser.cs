using PDDLParser.AST;
using PDDLParser.Domain;
using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser
{
    public class Parser
    {
        public DomainDecl ParseDomainFile(string parseFile)
        {
            string text = RemoveCommentsAndCombine(File.ReadAllLines(parseFile).ToList());
            text = text.ToLower();
            CheckParenthesesMissmatch(text);

            var absAST = ASTParse(text);

            string name = "";
            List<string> requirements = new List<string>();
            List<TypeDecl> types = new List<TypeDecl>();
            List<NameExp> constants = new List<NameExp>();
            List<PredicateStmt> predicates = new List<PredicateStmt>();
            List<ActionStmt> actions = new List<ActionStmt>();

            // Non-types parsing phase
            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("domain"))
                    name = PurgeEscapeChars(node.Content).Remove(0, "domain".Length).Trim();
                else if (node.Content.StartsWith(":requirements"))
                {
                    var str = PurgeEscapeChars(node.Content).Remove(0, ":requirements".Length).Trim();
                    var reqs = str.Split(' ');
                    foreach (var req in reqs)
                        if (req != "")
                            requirements.Add(req);
                }
            }

            // Type definition parsing
            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith(":types"))
                {
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
                }
            }

            // Post-type definition parsing
            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith(":constants"))
                {
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
                }
                else if (node.Content.StartsWith(":predicates"))
                {
                    foreach(var predicate in node.Children)
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
                        predicates.Add(new PredicateStmt(predicateName, argList));
                    }
                }
                else if (node.Content.StartsWith(":action"))
                {
                    var actionName = node.Content.Replace(":action", "").Split(' ')[0].Trim();

                }
            }

            return new DomainDecl(
                name,
                requirements,
                types,
                constants,
                predicates,
                actions
                );
        }

        private string PurgeEscapeChars(string str) => str.Replace("\r", "").Replace("\n", "").Replace("\t", "");

        private string RemoveCommentsAndCombine(List<string> lines)
        {
            string returnStr = "";
            foreach (var line in lines)
                if (!line.Trim().StartsWith(";"))
                    returnStr += line + "\n";
            return returnStr;
        }

        private ASTNode ASTParse(string text)
        {
            if (text.Count(x => x == ')') > 1)
            {
                var children = new List<ASTNode>();

                int thisP = text.IndexOf("(");

                while (text.Count(x => x == ')' || x == '(') > 2)
                {
                    int currentLevel = 0;
                    int startP = text.IndexOf("(", thisP + 1);
                    int endP = text.Length;
                    for (int i = startP + 1; i < text.Length; i++)
                    {
                        if (text[i] == '(')
                            currentLevel++;
                        if (text[i] == ')')
                        {
                            if (currentLevel == 0)
                            {
                                endP = i;
                                break;
                            }
                            currentLevel--;
                        }
                    }

                    children.Add(ASTParse(text.Substring(startP, endP - startP)));
                    text = text.Remove(startP, endP - startP + 1);
                }
                return new ASTNode(
                    text.Replace("(", "").Replace(")", "").Trim(),
                    children);
            }
            else
            {
                return new ASTNode(
                    text.Replace("(","").Replace(")","").Trim(),
                    new List<ASTNode>());
            }
        }

        private void CheckParenthesesMissmatch(string text)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                throw new ParseException(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParserErrorLevel.High,
                    ParseErrorCategory.Error,
                    - 1
                    );
            }
        }
    }
}
