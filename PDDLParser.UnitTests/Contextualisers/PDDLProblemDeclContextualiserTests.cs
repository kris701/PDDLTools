﻿using PDDLParser.AST;
using PDDLParser.Contextualisers;
using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
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
    public class PDDLProblemDeclContextualiserTests
    {
        [TestMethod]
        [DataRow("(define (:objects a) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a)))", "a", "")]
        [DataRow("(define (:objects a - type) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a)))", "a", "type")]
        [DataRow("(define (:objects a - q) (:init (pred ?a) (pred2 ?a)) (:goal (not (?a)))", "a", "q")]
        public void Can_DecorateObjectReferencesWithTypes(string toParse, string argName, string expectedType)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            ProblemDecl? decl = new ProblemVisitor().Visit(node, null, listener) as ProblemDecl;
            Assert.IsNotNull(decl);

            IContextualiser<ProblemDecl> contextualiser = new PDDLProblemDeclContextualiser();

            // ACT
            contextualiser.Contexturalise(decl, listener);

            // ASSERT
            foreach(var init in decl.Init.Predicates)
                Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(init, argName, expectedType));
            Assert.IsTrue(ContextualiserTestsHelpers.AreAllNameExpOfTypeOrSubType(decl.Goal.GoalExp, argName, expectedType));
        }
    }
}
