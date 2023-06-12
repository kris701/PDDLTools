using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Exceptions;
using PDDLParser.Listener;
using PDDLParser.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLParser.Models.Domain;

namespace PDDLParser
{
    public class Parser
    {
        public DomainDecl ParseDomainFile(string parseFile)
        {
            IErrorListener errorListener = new ErrorListener();
            errorListener.ThrowIfTypeAbove = ParseErrorType.Warning;

            var text = ReadDataAsString(parseFile, errorListener);
            CheckParenthesesMissmatch(text, errorListener);

            var astParser = new ASTParser(errorListener);
            var absAST = astParser.Parse(text);

            var returnDomain = new DomainDecl();

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("domain"))
                    returnDomain.Name = DomainVisitor.Visit(node, errorListener) as DomainNameDecl;
                else if (node.Content.StartsWith(":requirements"))
                    returnDomain.Requirements = DomainVisitor.Visit(node, errorListener) as RequirementsDecl;
                if (node.Content.StartsWith(":types"))
                    returnDomain.Types = DomainVisitor.Visit(node, errorListener) as TypesDecl;
                if (node.Content.StartsWith(":constants"))
                    returnDomain.Constants = DomainVisitor.Visit(node, errorListener) as ConstantsDecl;
                if (node.Content.StartsWith(":predicates"))
                    returnDomain.Predicates = DomainVisitor.Visit(node, errorListener) as PredicatesDecl;
                if (node.Content.StartsWith(":action"))
                {
                    if (returnDomain.Actions == null)
                        returnDomain.Actions = new List<ActionDecl>();
                    returnDomain.Actions.Add(DomainVisitor.Visit(node, errorListener) as ActionDecl);
                }
            }

            return returnDomain;
        }

        private string ReadDataAsString(string path, IErrorListener listener)
        {
            if (!File.Exists(path))
            {
                listener.AddError(new ParseError(
                    $"Could not find the file to parse: '{path}'",
                    ParserErrorLevel.High,
                    ParseErrorType.Error));
            }
            string text = ReplaceCommentsWithWhiteSpace(File.ReadAllLines(path).ToList());
            text = text.ToLower();
            return text;
        }

        private string ReplaceCommentsWithWhiteSpace(List<string> lines)
        {
            string returnStr = "";
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith(";"))
                {
                    returnStr += new string(' ', line.Length) + "\n";
                }
                else
                    returnStr += line + "\n";
            }
            return returnStr;
        }

        private void CheckParenthesesMissmatch(string text, IErrorListener listener)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                listener.AddError(new ParseError(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParserErrorLevel.High,
                    ParseErrorType.Error));
            }
        }
    }
}
