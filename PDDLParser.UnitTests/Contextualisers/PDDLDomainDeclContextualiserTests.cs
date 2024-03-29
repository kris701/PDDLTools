﻿using PDDLParser.AST;
using PDDLParser.Contextualisers;
using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.UnitTests.Contextualisers
{
    [TestClass]
    public class PDDLDomainDeclContextualiserTests
    {
        [TestMethod]
        [DataRow("(define (:action name :parameters (?a) :precondition (p a) :effect (p a)))", "?a", "")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (p a) :effect (p a)))", "?a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (p a)) :effect (p a)))", "?a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (p a)) :effect (and (p a))))", "?a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (p a)) :effect (or (p a) (not (p a)))))", "?a", "type")]
        public void Can_DecorateActionParameterReferencesWithType(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            DomainDecl? decl = new DomainVisitor().Visit(node, null, listener) as DomainDecl;
            Assert.IsNotNull(decl);

            IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser();

            // ACT
            contextualiser.Contexturalise(decl, listener);

            // ASSERT
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Actions[0].Preconditions, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Actions[0].Effects, argName, expectedType));
        }

        [TestMethod]
        [DataRow("(define (:axiom :vars (?a) :context (p a) :implies (p a)))", "?a", "")]
        [DataRow("(define (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (p a)) :implies (p a)))", "?a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (p a)) :implies (and (p a))))", "?a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (p a)) :implies (or (p a) (not (p a)))))", "?a", "type")]
        public void Can_DecorateAxiomParameterReferencesWithType(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            DomainDecl? decl = new DomainVisitor().Visit(node, null, listener) as DomainDecl;
            Assert.IsNotNull(decl);

            IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser();

            // ACT
            contextualiser.Contexturalise(decl, listener);

            // ASSERT
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Context, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Implies, argName, expectedType));
        }

        [TestMethod]
        [DataRow("(define (:types type - supertype) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "supertype")]
        [DataRow("(define (:types type - other \n supertype - highersupertype) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "other")]
        [DataRow("(define (:types type - other \n supertype - type) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "other")]
        [DataRow("(define (:types type) (:axiom :vars (?a - type) :context (p a) :implies (p a)))", "?a", "")]
        public void Can_CanDecorateSubtypes(string toParse, string argName, string expectedSuperType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            DomainDecl? decl = new DomainVisitor().Visit(node, null, listener) as DomainDecl;
            Assert.IsNotNull(decl);

            IContextualiser<DomainDecl> contextualiser = new PDDLDomainDeclContextualiser();

            // ACT
            contextualiser.Contexturalise(decl, listener);

            // ASSERT
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Context, argName, expectedSuperType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Axioms[0].Implies, argName, expectedSuperType));
        }
    }
}
