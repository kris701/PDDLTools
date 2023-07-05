using PDDLParser.AST;
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

namespace PDDLParser.Tests.Visitors
{
    [TestClass]
    public class ProblemVisitorTests
    {
        [TestMethod]
        [DataRow("(define (problem a))", typeof(ProblemDecl))]
        [DataRow("(problem abc)", typeof(ProblemNameDecl))]
        [DataRow("(:objects abc def - type)", typeof(ObjectsDecl))]
        [DataRow("(:init (a ?b) (c ?d))", typeof(InitDecl))]
        [DataRow("(:goal (not (a)))", typeof(GoalDecl))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            var decl = ProblemVisitor.Visit(node, null, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, expectedType);
        }

        [TestMethod]
        [DataRow("(define)")]
        [DataRow("(define (problem a))")]
        public void Can_VisitProblemDecl(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            ProblemVisitor.TryVisitProblemDeclNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ProblemDecl));
        }

        [TestMethod]
        [DataRow("(define abava)")]
        [DataRow("(define a)")]
        public void Cant_VisitProblemDecl_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            ProblemVisitor.TryVisitProblemDeclNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(problem)")]
        [DataRow("(problem abc)")]
        public void Can_VisitProblemNameNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            ProblemVisitor.TryVisitProblemNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ProblemNameDecl));
        }

        [TestMethod]
        [DataRow("(:objects)")]
        [DataRow("(:objects abc)")]
        [DataRow("(:objects abc - type)")]
        [DataRow("(:objects abc def - type)")]
        public void Can_VisitObjectsNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            ProblemVisitor.TryVisitObjectsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ObjectsDecl));
        }

        [TestMethod]
        [DataRow("(:init)")]
        [DataRow("(:init (a))")]
        [DataRow("(:init (a ?b))")]
        [DataRow("(:init (a ?b) (c ?d))")]
        public void Can_VisitInitsNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            ProblemVisitor.TryVisitInitsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(InitDecl));
        }

        [TestMethod]
        [DataRow("(:goal ())")]
        [DataRow("(:goal (a))")]
        [DataRow("(:goal (not (a)))")]
        public void Can_VisitGoalsNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            ProblemVisitor.TryVisitGoalNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(GoalDecl));
        }
    }
}
