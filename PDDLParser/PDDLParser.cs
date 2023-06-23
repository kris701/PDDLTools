﻿using PDDLParser.AST;
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

        public PDDLParser(bool contextualise = true)
        {
            Listener = new ErrorListener();
            Listener.ThrowIfTypeAbove = ParseErrorType.Warning;
            Contextualise = contextualise;
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

                    IAnalyser<PDDLDecl> pddlAnalyser = new PDDLDeclAnalyser();
                    pddlAnalyser.PostAnalyse(decl, Listener);
                }
                else if (domainFile != null)
                {
                    IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser();
                    contextualiser.Contexturalise(decl.Domain, Listener);

                    IAnalyser<DomainDecl> domainAnalyser = new DomainAnalyser();
                    domainAnalyser.PostAnalyse(decl.Domain, Listener);
                }
                else if (problemFile != null)
                {
                    IContextualiser<ProblemDecl> contextualiser = new PDDLProblemDeclContextualiser();
                    contextualiser.Contexturalise(decl.Problem, Listener);

                    IAnalyser<ProblemDecl> problemAnalyser = new ProblemAnalyser();
                    problemAnalyser.PostAnalyse(decl.Problem, Listener);
                }
            }

            return decl;
        }

        private DomainDecl ParseDomain(string parseFile)
        {
            if (!PDDLHelper.IsFileDomain(parseFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a domain file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            var absAST = ParseAsASTTree(parseFile, Listener);

            if (!absAST.Content.StartsWith("define"))
                Listener.AddError(new ParseError(
                    $"Root 'define' node not found?",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    absAST.Line,
                    absAST.Character));

            IsChildrenOnly(absAST, "define");

            var returnDomain = new DomainDecl(absAST);

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("domain"))
                    returnDomain.Name = DomainVisitor.Visit(node, Listener) as DomainNameDecl;
                else if (node.Content.StartsWith(":requirements"))
                    returnDomain.Requirements = DomainVisitor.Visit(node, Listener) as RequirementsDecl;
                else if (node.Content.StartsWith(":extends"))
                    returnDomain.Extends = DomainVisitor.Visit(node, Listener) as ExtendsDecl;
                else if (node.Content.StartsWith(":types"))
                    returnDomain.Types = DomainVisitor.Visit(node, Listener) as TypesDecl;
                else if (node.Content.StartsWith(":constants"))
                    returnDomain.Constants = DomainVisitor.Visit(node, Listener) as ConstantsDecl;
                else if (node.Content.StartsWith(":timeless"))
                    returnDomain.Timeless = DomainVisitor.Visit(node, Listener) as TimelessDecl;
                else if (node.Content.StartsWith(":predicates"))
                {
                    if (IsChildrenOnly(node, ":predicates"))
                        returnDomain.Predicates = DomainVisitor.Visit(node, Listener) as PredicatesDecl;
                }
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
                else
                    Listener.AddError(new ParseError(
                        $"Could not parse content of AST node: {node.Content}",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Character));
            }

            return returnDomain;
        }

        private ProblemDecl ParseProblem(string parseFile)
        {
            if (!PDDLHelper.IsFileProblem(parseFile))
                Listener.AddError(new ParseError(
                    $"Attempted file to parse was not a problem file!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));

            var absAST = ParseAsASTTree(parseFile, Listener);

            if (!absAST.Content.StartsWith("define"))
                Listener.AddError(new ParseError(
                    $"Root 'define' node not found?",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    absAST.Line,
                    absAST.Character));

            IsChildrenOnly(absAST, "define");

            var returnProblem = new ProblemDecl(absAST);

            foreach (var node in absAST.Children)
            {
                if (node.Content.StartsWith("problem"))
                    returnProblem.Name = ProblemVisitor.Visit(node, Listener) as ProblemNameDecl;
                else if (node.Content.StartsWith(":domain"))
                    returnProblem.DomainName = ProblemVisitor.Visit(node, Listener) as DomainNameRefDecl;
                else if (node.Content.StartsWith(":objects"))
                    returnProblem.Objects = ProblemVisitor.Visit(node, Listener) as ObjectsDecl;
                else if (node.Content.StartsWith(":init")) {
                    if (IsChildrenOnly(node, ":init"))
                        returnProblem.Init = ProblemVisitor.Visit(node, Listener) as InitDecl;
                }
                else if (node.Content.StartsWith(":goal")) {
                    if (IsChildrenOnly(node, ":goal"))
                        returnProblem.Goal = ProblemVisitor.Visit(node, Listener) as GoalDecl;
                }
                else
                    Listener.AddError(new ParseError(
                        $"Could not parse content of AST node: {node.Content}",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Character));
            }

            return returnProblem;
        }

        private bool IsChildrenOnly(ASTNode node, string targetName)
        {
            if (node.Content.Replace(targetName, "").Trim() != "")
            {
                Listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.Content.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Character));
                return false;
            }
            return true;
        }

        private ASTNode ParseAsASTTree(string path, IErrorListener listener)
        {
            var text = ReadDataAsString(path, listener);
            var analyser = new GeneralPreAnalyser();
            analyser.PreAnalyse(text, listener);

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
