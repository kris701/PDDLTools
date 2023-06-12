using PDDLParser.AST;
using PDDLParser.Domain;
using PDDLParser.Exceptions;
using PDDLParser.Visitors;
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

            var astParser = new ASTParser();
            var absAST = astParser.ASTParse(text);

            var returnDomain = new DomainDecl();

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("domain"))
                    returnDomain.Name = DeclVisitor.Visit(node) as DomainNameDecl;
                else if (node.Content.StartsWith(":requirements"))
                    returnDomain.Requirements = DeclVisitor.Visit(node) as RequirementsDecl;
                if (node.Content.StartsWith(":types"))
                    returnDomain.Types = DeclVisitor.Visit(node) as TypesDecl;
                if (node.Content.StartsWith(":constants"))
                    returnDomain.Constants = DeclVisitor.Visit(node) as ConstantsDecl;
                if (node.Content.StartsWith(":predicates"))
                    returnDomain.Predicates = DeclVisitor.Visit(node) as PredicatesDecl;
                if (node.Content.StartsWith(":action"))
                {
                    if (returnDomain.Actions == null)
                        returnDomain.Actions = new List<ActionDecl>();
                    returnDomain.Actions.Add(DeclVisitor.Visit(node) as ActionDecl);
                }
            }

            return returnDomain;
        }

        private string RemoveCommentsAndCombine(List<string> lines)
        {
            string returnStr = "";
            foreach (var line in lines)
                if (!line.Trim().StartsWith(";"))
                    returnStr += line + "\n";
            return returnStr;
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
