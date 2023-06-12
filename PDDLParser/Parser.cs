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
using PDDLParser.Models.Problem;

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
                else if (node.Content.StartsWith(":types"))
                    returnDomain.Types = DomainVisitor.Visit(node, errorListener) as TypesDecl;
                else if (node.Content.StartsWith(":constants"))
                    returnDomain.Constants = DomainVisitor.Visit(node, errorListener) as ConstantsDecl;
                else if (node.Content.StartsWith(":timeless"))
                    returnDomain.Timeless = DomainVisitor.Visit(node, errorListener) as TimelessDecl;
                else if (node.Content.StartsWith(":predicates"))
                    returnDomain.Predicates = DomainVisitor.Visit(node, errorListener) as PredicatesDecl;
                else if (node.Content.StartsWith(":action"))
                {
                    if (returnDomain.Actions == null)
                        returnDomain.Actions = new List<ActionDecl>();
                    returnDomain.Actions.Add(DomainVisitor.Visit(node, errorListener) as ActionDecl);
                }
                else if (node.Content.StartsWith(":axiom"))
                {
                    if (returnDomain.Axioms == null)
                        returnDomain.Axioms = new List<AxiomDecl>();
                    returnDomain.Axioms.Add(DomainVisitor.Visit(node, errorListener) as AxiomDecl);
                }
            }

            return returnDomain;
        }

        public ProblemDecl ParseProblemFile(string parseFile)
        {
            IErrorListener errorListener = new ErrorListener();
            errorListener.ThrowIfTypeAbove = ParseErrorType.Warning;

            var text = ReadDataAsString(parseFile, errorListener);
            CheckParenthesesMissmatch(text, errorListener);

            var astParser = new ASTParser(errorListener);
            var absAST = astParser.Parse(text);

            var returnProblem = new ProblemDecl();

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("problem"))
                    returnProblem.Name = ProblemVisitor.Visit(node, errorListener) as ProblemNameDecl;
                else if (node.Content.StartsWith(":domain"))
                    returnProblem.DomainName = ProblemVisitor.Visit(node, errorListener) as DomainNameRefDecl;
                if (node.Content.StartsWith(":objects"))
                    returnProblem.Objects = ProblemVisitor.Visit(node, errorListener) as ObjectsDecl;
                if (node.Content.StartsWith(":init"))
                    returnProblem.Init = ProblemVisitor.Visit(node, errorListener) as InitDecl;
                if (node.Content.StartsWith(":goal"))
                    returnProblem.Goal = ProblemVisitor.Visit(node, errorListener) as GoalDecl;
            }

            return returnProblem;
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
