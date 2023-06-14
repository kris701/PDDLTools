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
using PDDLParser.Analysers;

namespace PDDLParser
{
    public class PDDLParser : IPDDLParser
    {
        public IErrorListener Listener { get; }

        public PDDLParser()
        {
            Listener = new ErrorListener();
            Listener.ThrowIfTypeAbove = ParseErrorType.Warning;
        }

        public PDDLDecl ParseDomainAndProblemFiles(string domainFile, string problemFile)
        {
            return new PDDLDecl(
                ParseDomainFile(domainFile),
                ParseProblemFile(problemFile));
        }

        public DomainDecl ParseDomainFile(string parseFile)
        {
            var absAST = ParseAsASTTree(parseFile, Listener);

            var returnDomain = new DomainDecl(absAST);

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("domain"))
                    returnDomain.Name = DomainVisitor.Visit(node, Listener) as DomainNameDecl;
                else if (node.Content.StartsWith(":requirements"))
                    returnDomain.Requirements = DomainVisitor.Visit(node, Listener) as RequirementsDecl;
                else if (node.Content.StartsWith(":types"))
                    returnDomain.Types = DomainVisitor.Visit(node, Listener) as TypesDecl;
                else if (node.Content.StartsWith(":constants"))
                    returnDomain.Constants = DomainVisitor.Visit(node, Listener) as ConstantsDecl;
                else if (node.Content.StartsWith(":timeless"))
                    returnDomain.Timeless = DomainVisitor.Visit(node, Listener) as TimelessDecl;
                else if (node.Content.StartsWith(":predicates"))
                    returnDomain.Predicates = DomainVisitor.Visit(node, Listener) as PredicatesDecl;
                else if (node.Content.StartsWith(":action"))
                {
                    if (returnDomain.Actions == null)
                        returnDomain.Actions = new List<ActionDecl>();
                    returnDomain.Actions.Add(DomainVisitor.Visit(node, Listener) as ActionDecl);
                }
                else if (node.Content.StartsWith(":axiom"))
                {
                    if (returnDomain.Axioms == null)
                        returnDomain.Axioms = new List<AxiomDecl>();
                    returnDomain.Axioms.Add(DomainVisitor.Visit(node, Listener) as AxiomDecl);
                }
            }

            PostParsingAnalyser.AnalyseDomain(returnDomain, Listener);

            return returnDomain;
        }

        public ProblemDecl ParseProblemFile(string parseFile)
        {
            var absAST = ParseAsASTTree(parseFile, Listener);

            var returnProblem = new ProblemDecl(absAST);

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("problem"))
                    returnProblem.Name = ProblemVisitor.Visit(node, Listener) as ProblemNameDecl;
                else if (node.Content.StartsWith(":domain"))
                    returnProblem.DomainName = ProblemVisitor.Visit(node, Listener) as DomainNameRefDecl;
                if (node.Content.StartsWith(":objects"))
                    returnProblem.Objects = ProblemVisitor.Visit(node, Listener) as ObjectsDecl;
                if (node.Content.StartsWith(":init"))
                    returnProblem.Init = ProblemVisitor.Visit(node, Listener) as InitDecl;
                if (node.Content.StartsWith(":goal"))
                    returnProblem.Goal = ProblemVisitor.Visit(node, Listener) as GoalDecl;
            }

            PostParsingAnalyser.AnalyseProblem(returnProblem, Listener);

            return returnProblem;
        }

        private ASTNode ParseAsASTTree(string path, IErrorListener listener)
        {
            var text = ReadDataAsString(path, listener);
            PreParsingAnalyser.AnalyseText(text, listener);

            var astParser = new ASTParser(listener);
            var absAST = astParser.Parse(text);
            return absAST;
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
    }
}
