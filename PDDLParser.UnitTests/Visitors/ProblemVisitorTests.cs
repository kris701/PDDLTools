﻿using PDDLParser.AST;
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

namespace PDDLParser.UnitTests.Visitors
{
    [TestClass]
    public class ProblemVisitorTests
    {
        [TestMethod]
        [DataRow("(define (problem a))", typeof(ProblemDecl))]
        [DataRow("(problem abc)", typeof(ProblemNameDecl))]
        [DataRow("(:domain abc)", typeof(DomainNameRefDecl))]
        [DataRow("(:objects abc def - type)", typeof(ObjectsDecl))]
        [DataRow("(:init (a ?b) (c ?d))", typeof(InitDecl))]
        [DataRow("(:goal (not (a)))", typeof(GoalDecl))]
        [DataRow("(:metric maximize (= (something) 10))", typeof(MetricDecl))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            var decl = new ProblemVisitor().Visit(node, null, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, expectedType);
        }

        [TestMethod]
        [DataRow("(define)")]
        [DataRow("(define (problem a))")]
        public void Can_VisitProblemDecl(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitProblemDeclNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ProblemDecl));
        }

        [TestMethod]
        [DataRow("(define abava)")]
        [DataRow("(define a)")]
        public void Cant_VisitProblemDecl_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitProblemDeclNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(problem abc)")]
        [DataRow("(problem    abc)")]
        [DataRow("(problem    abc       )")]
        public void Can_VisitProblemNameNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitProblemNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ProblemNameDecl));
        }

        [TestMethod]
        [DataRow("(problem abc)", "abc")]
        [DataRow("(problem a)", "a")]
        [DataRow("(problem    abc)", "abc")]
        [DataRow("(problem    abc       )", "abc")]
        [DataRow("(problem    abcaaaaaaaaaaaaaaaa       )", "abcaaaaaaaaaaaaaaaa")]
        public void Can_VisitProblemNameNode_CorrectName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitProblemNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ProblemNameDecl));
            if (decl is ProblemNameDecl name)
                Assert.AreEqual(expected, name.Name);
        }

        [TestMethod]
        [DataRow("(problem)")]
        [DataRow("(problem         )")]
        public void Cant_VisitProblemNameNode_IfNoLooseChild(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitProblemNameNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactLooseChildren);
        }

        [TestMethod]
        [DataRow("(:domain abc)")]
        [DataRow("(:domain    abc)")]
        [DataRow("(:domain    abc       )")]
        public void Can_VisitDomainRefNameNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitDomainRefNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainNameRefDecl));
        }

        [TestMethod]
        [DataRow("(:domain abc)", "abc")]
        [DataRow("(:domain a)", "a")]
        [DataRow("(:domain    abc)", "abc")]
        [DataRow("(:domain    abc       )", "abc")]
        [DataRow("(:domain    abcaaaaaaaaaaaaaaaa       )", "abcaaaaaaaaaaaaaaaa")]
        public void Can_VisitDomainRefNameNode_CoorectName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitDomainRefNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainNameRefDecl));
            if (decl is DomainNameRefDecl name)
                Assert.AreEqual(expected, name.Name);
        }

        [TestMethod]
        [DataRow("(:domain)")]
        [DataRow("(:domain         )")]
        public void Cant_VisitDomainRefNameNode_IfNoLooseChild(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitDomainRefNameNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactLooseChildren);
        }

        [TestMethod]
        [DataRow("(:objects)")]
        [DataRow("(:objects abc)")]
        [DataRow("(:objects abc - type)")]
        [DataRow("(:objects abc def - type)")]
        public void Can_VisitObjectsNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitObjectsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ObjectsDecl));
        }

        [TestMethod]
        [DataRow("(:objects a)", "a")]
        [DataRow("(:objects a b)", "a", "b")]
        [DataRow("(:objects a b - type)", "a", "b")]
        [DataRow("(:objects a longName b)", "a", "longName", "b")]
        public void Can_VisitObjectsNode_CorrectNames(string toParse, params string[] expObjNames)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitObjectsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ObjectsDecl));
            if (decl is ObjectsDecl objs)
            {
                Assert.AreEqual(expObjNames.Length, objs.Objs.Count);
                for (int i = 0; i < objs.Objs.Count; i++)
                    Assert.AreEqual(expObjNames[i], objs.Objs[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:objects a)", "")]
        [DataRow("(:objects a b)", "", "")]
        [DataRow("(:objects a - type b - type)", "type", "type")]
        [DataRow("(:objects a - type2 b - type)", "type2", "type")]
        [DataRow("(:objects a - type \n longName - type2 b - type2)", "type", "type2", "type2")]
        public void Can_VisitObjectsNode_CorrectTypes(string toParse, params string[] expObjType)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitObjectsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ObjectsDecl));
            if (decl is ObjectsDecl objs)
            {
                Assert.AreEqual(expObjType.Length, objs.Objs.Count);
                for (int i = 0; i < objs.Objs.Count; i++)
                    Assert.AreEqual(expObjType[i], objs.Objs[i].Type.Name);
            }
        }

        [TestMethod]
        [DataRow("(:objects ())")]
        [DataRow("(:objects () a ())")]
        public void Cant_VisitObjectsNode_IfContainsChildren(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitObjectsNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NoChildrenAllowed);
        }

        [TestMethod]
        [DataRow("(:init)")]
        [DataRow("(:init (a))")]
        [DataRow("(:init (a ?b))")]
        [DataRow("(:init (a ?b) (c ?d))")]
        public void Can_VisitInitsNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitInitsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(InitDecl));
        }

        [TestMethod]
        [DataRow("(:init (a))", "a")]
        [DataRow("(:init (a ?b))", "a")]
        [DataRow("(:init (a ?b) (c ?d))", "a", "c")]
        [DataRow("(:init (aasfg ?b) (c ?d))", "aasfg", "c")]
        public void Can_VisitInitsNode_CorrectPredicates(string toParse, params string[] expPredi)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitInitsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(InitDecl));
            if (decl is InitDecl inits)
            {
                Assert.AreEqual(expPredi.Length, inits.Predicates.Count);
                for (int i = 0; i < inits.Predicates.Count; i++)
                    Assert.AreEqual(expPredi[i], (inits.Predicates[i] as PredicateExp).Name);
            }
        }

        [TestMethod]
        [DataRow("(:init a())")]
        [DataRow("(:init a() abdad ())")]
        public void Cant_VisitInitNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitInitsNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(:goal ())")]
        [DataRow("(:goal (a))")]
        [DataRow("(:goal (not (a)))")]
        public void Can_VisitGoalsNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitGoalNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(GoalDecl));
        }

        [TestMethod]
        [DataRow("(:goal (a))", "a")]
        [DataRow("(:goal (abcd ?a))", "abcd")]
        public void Can_VisitGoalsNode_CorrectNode(string toParse, string node1)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitGoalNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(GoalDecl));
            if (decl is GoalDecl goal)
            {
                Assert.IsInstanceOfType(goal.GoalExp, typeof(PredicateExp));
                if (goal.GoalExp is PredicateExp exp)
                    Assert.AreEqual(node1, exp.Name);
            }
        }

        [TestMethod]
        [DataRow("(:goal () () ())")]
        [DataRow("(:goal () ())")]
        [DataRow("(:goal)")]
        public void Cant_VisitGoalNode_IfNotHaveingSingleChild(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitGoalNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactChildren);
        }


        [TestMethod]
        [DataRow("(:goal a())")]
        [DataRow("(:goal a() abdad)")]
        public void Cant_VisitGoalNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitGoalNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(:metric maximize ())")]
        [DataRow("(:metric minimize (a))")]
        [DataRow("(:metric maximize (= (a) 10))")]
        public void Can_VisitMetricNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            new ProblemVisitor().TryVisitMetricNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(MetricDecl));
        }
    }
}
