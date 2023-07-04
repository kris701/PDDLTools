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
using System.Xml.Linq;
using PDDLParser.Helpers;
using PDDLParser.Contextualisers;

namespace PDDLParser
{
    public class PDDLParser : IPDDLParser
    {
        public IErrorListener Listener { get; }
        public bool Contextualise { get; set; }
        public bool Analyse { get; set; }

        public PDDLParser(bool contextualise = true, bool analyse = true)
        {
            Listener = new ErrorListener();
            Listener.ThrowIfTypeAbove = ParseErrorType.Warning;
            Contextualise = contextualise;
            Analyse = analyse;
        }

        public PDDLDecl Parse(string domainFile = null, string problemFile = null)
        {
            var decl = new PDDLDecl(
                ParseDomain(domainFile),
                ParseProblem(problemFile));

            if (Contextualise)
            {
                if (domainFile != null && problemFile != null)
                {
                    IContextualiser<PDDLDecl> contextualiser = new PDDLDeclContextualiser();
                    contextualiser.Contexturalise(decl, Listener);
                }
                else if (domainFile != null)
                {
                    IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser();
                    contextualiser.Contexturalise(decl.Domain, Listener);
                }
                else if (problemFile != null)
                {
                    IContextualiser<ProblemDecl> contextualiser = new PDDLProblemDeclContextualiser();
                    contextualiser.Contexturalise(decl.Problem, Listener);
                }
            }

            if (Analyse)
            {
                if (domainFile != null && problemFile != null)
                {
                    IAnalyser<PDDLDecl> pddlAnalyser = new PDDLDeclAnalyser();
                    pddlAnalyser.PostAnalyse(decl, Listener);
                }
                else if (domainFile != null)
                {
                    IAnalyser<DomainDecl> domainAnalyser = new PDDLDomainDeclAnalyser();
                    domainAnalyser.PostAnalyse(decl.Domain, Listener);
                }
                else if (problemFile != null)
                {
                    IAnalyser<ProblemDecl> problemAnalyser = new PDDLProblemDeclAnalyser();
                    problemAnalyser.PostAnalyse(decl.Problem, Listener);
                }
            }


            return decl;
        }

        private DomainDecl ParseDomain(string parseFile)
        {
            if (parseFile == null)
                return new DomainDecl(new ASTNode());

            if (!PDDLHelper.IsFileDomain(parseFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a domain file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            var absAST = ParseAsASTTree(parseFile, Listener);

            if (!absAST.OuterContent.StartsWith("define"))
                Listener.AddError(new ParseError(
                    $"Root 'define' node not found?",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    absAST.Line,
                    absAST.Start));

            IsChildrenOnly(absAST, "define");

            var returnDomain = new DomainDecl(absAST);

            foreach (var node in absAST.Children)
            {
                if (node.OuterContent.StartsWith("domain"))
                    returnDomain.Name = DomainVisitor.Visit(node, returnDomain, Listener) as DomainNameDecl;
                else if (node.OuterContent.StartsWith(":requirements"))
                    returnDomain.Requirements = DomainVisitor.Visit(node, returnDomain, Listener) as RequirementsDecl;
                else if (node.OuterContent.StartsWith(":extends"))
                    returnDomain.Extends = DomainVisitor.Visit(node, returnDomain, Listener) as ExtendsDecl;
                else if (node.OuterContent.StartsWith(":types"))
                    returnDomain.Types = DomainVisitor.Visit(node, returnDomain, Listener) as TypesDecl;
                else if (node.OuterContent.StartsWith(":constants"))
                    returnDomain.Constants = DomainVisitor.Visit(node, returnDomain, Listener) as ConstantsDecl;
                else if (node.OuterContent.StartsWith(":timeless"))
                    returnDomain.Timeless = DomainVisitor.Visit(node, returnDomain, Listener) as TimelessDecl;
                else if (node.OuterContent.StartsWith(":predicates"))
                {
                    if (IsChildrenOnly(node, ":predicates"))
                        returnDomain.Predicates = DomainVisitor.Visit(node, returnDomain, Listener) as PredicatesDecl;
                }
                else if (node.OuterContent.StartsWith(":action"))
                {
                    if (returnDomain.Actions == null)
                        returnDomain.Actions = new List<ActionDecl>();
                    returnDomain.Actions.Add(DomainVisitor.Visit(node, returnDomain, Listener) as ActionDecl);
                }
                else if (node.OuterContent.StartsWith(":axiom"))
                {
                    if (returnDomain.Axioms == null)
                        returnDomain.Axioms = new List<AxiomDecl>();
                    returnDomain.Axioms.Add(DomainVisitor.Visit(node, returnDomain, Listener) as AxiomDecl);
                }
                else
                    Listener.AddError(new ParseError(
                        $"Could not parse content of AST node: {node.OuterContent}",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
            }

            return returnDomain;
        }

        private ProblemDecl ParseProblem(string parseFile)
        {
            if (parseFile == null)
                return new ProblemDecl(new ASTNode());

            if (!PDDLHelper.IsFileProblem(parseFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a problem file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            var absAST = ParseAsASTTree(parseFile, Listener);

            if (!absAST.OuterContent.StartsWith("define"))
                Listener.AddError(new ParseError(
                    $"Root 'define' node not found?",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    absAST.Line,
                    absAST.Start));

            IsChildrenOnly(absAST, "define");

            var returnProblem = new ProblemDecl(absAST);

            foreach (var node in absAST.Children)
            {
                if (node.OuterContent.StartsWith("problem"))
                    returnProblem.Name = ProblemVisitor.Visit(node, returnProblem, Listener) as ProblemNameDecl;
                else if (node.OuterContent.StartsWith(":domain"))
                    returnProblem.DomainName = ProblemVisitor.Visit(node, returnProblem, Listener) as DomainNameRefDecl;
                else if (node.OuterContent.StartsWith(":objects"))
                    returnProblem.Objects = ProblemVisitor.Visit(node, returnProblem, Listener) as ObjectsDecl;
                else if (node.OuterContent.StartsWith(":init")) {
                    if (IsChildrenOnly(node, ":init"))
                        returnProblem.Init = ProblemVisitor.Visit(node, returnProblem, Listener) as InitDecl;
                }
                else if (node.OuterContent.StartsWith(":goal")) {
                    if (IsChildrenOnly(node, ":goal"))
                        returnProblem.Goal = ProblemVisitor.Visit(node, returnProblem, Listener) as GoalDecl;
                }
                else
                    Listener.AddError(new ParseError(
                        $"Could not parse content of AST node: {node.OuterContent}",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
            }

            return returnProblem;
        }

        private bool IsChildrenOnly(ASTNode node, string targetName)
        {
            if (node.OuterContent.Replace(targetName, "").Trim() != "")
            {
                Listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.OuterContent.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        private ASTNode ParseAsASTTree(string path, IErrorListener listener)
        {
            var text = ReadDataAsString(path, listener);
            var analyser = new GeneralPreAnalyser();
            analyser.PreAnalyse(text, listener);

            var astParser = new ASTParser();
            var absAST = astParser.Parse(text);
            return absAST;
        }

        private string ReadDataAsString(string path, IErrorListener listener)
        {
            if (!File.Exists(path))
            {
                listener.AddError(new ParseError(
                    $"Could not find the file to parse: '{path}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));
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
