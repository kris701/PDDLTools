using PDDLParser.AST;
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

namespace PDDLParser.Tests.Contextualisers
{
    [TestClass]
    public class PDDLDomainDeclContextualiserTests
    {
        [TestMethod]
        [DataRow("(define (:action name :parameters (?a) :precondition (a) :effect (a)))", "a", "")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (a) :effect (a)))", "a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (a)) :effect (a)))", "a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (a)) :effect (and (a))))", "a", "type")]
        [DataRow("(define (:action name :parameters (?a - type) :precondition (not (a)) :effect (or (a) (not (a)))))", "a", "type")]
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
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfType(decl.Actions[0].Preconditions, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfType(decl.Actions[0].Effects, argName, expectedType));
        }

        [TestMethod]
        [DataRow("(define (:axiom :vars (?a) :context (a) :implies (a)))", "a", "")]
        [DataRow("(define (:axiom :vars (?a - type) :context (a) :implies (a)))", "a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (a)) :implies (a)))", "a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (a)) :implies (and (a))))", "a", "type")]
        [DataRow("(define (:axiom :vars (?a - type) :context (not (a)) :implies (or (a) (not (a)))))", "a", "type")]
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
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfType(decl.Axioms[0].Context, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfType(decl.Axioms[0].Implies, argName, expectedType));
        }
    }
}
