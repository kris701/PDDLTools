using PDDLParser.AST;
using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Tests.Visitors
{
    [TestClass]
    public class ExpVisitorTests
    {
        [TestMethod]
        [DataRow("(and ())", typeof(AndExp))]
        [DataRow("(not (aaabsbdsb))", typeof(NotExp))]
        [DataRow("(or (aaa) (bbb))", typeof(OrExp))]
        [DataRow("(pred)", typeof(PredicateExp))]
        [DataRow("(pred ?a)", typeof(PredicateExp))]
        [DataRow("name=t=type", typeof(NameExp))]
        [DataRow("name", typeof(NameExp))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            var exp = ExpVisitor.Visit(node, null, null);

            // ASSERT
            Assert.IsInstanceOfType(exp, expectedType);
        }

        [TestMethod]
        [DataRow("(and ())")]
        [DataRow("(and () () ())")]
        [DataRow("(and (abba) (aaa) (qwrer))")]
        public void Can_ParseAndNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitAndNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(AndExp));
        }

        [TestMethod]
        [DataRow("(and)")]
        [DataRow("(and         )")]
        public void Cant_ParseAndNode_IfNoChildren(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            ExpVisitor.TryVisitAndNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.MustHaveMoreThanChildren);
        }

        [TestMethod]
        [DataRow("(and () q)")]
        [DataRow("(and       ()   b  )")]
        [DataRow("(and  () qqfa     ()   b  )")]
        public void Cant_ParseAndNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            ExpVisitor.TryVisitAndNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(or () ())")]
        [DataRow("(or (abcas) (qwefas))")]
        public void Can_ParseOrNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitOrNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(OrExp));
        }

        [TestMethod]
        [DataRow("(or () () ())")]
        [DataRow("(or ())")]
        [DataRow("(or)")]
        [DataRow("(or (abcas) () (qwefas))")]
        public void Cant_ParseOrNode_IfNotExactChildCount(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            ExpVisitor.TryVisitOrNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactChildren);
        }

        [TestMethod]
        [DataRow("(or () a ())")]
        [DataRow("(or () a () bafras)")]
        [DataRow("(or  aaa()           a () bafras)")]
        public void Cant_ParseOrNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            ExpVisitor.TryVisitOrNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(not ())")]
        [DataRow("(not (abcas))")]
        public void Can_ParseNotNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNotNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NotExp));
        }

        [TestMethod]
        [DataRow("(not)")]
        [DataRow("(not            )")]
        public void Cant_ParseNotNode_IfNoChild(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNotNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactChildren);
        }

        [TestMethod]
        [DataRow("(not () a)")]
        [DataRow("(not     a   ()       )")]
        [DataRow("(not     a   ()     dsgsdgdsg  )")]
        public void Cant_ParseNotNode_IfContainsStrayCharacters(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNotNode(node, null, listener, out exp);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(pred)")]
        [DataRow("(pred ?a)")]
        [DataRow("(pred ?a ?b)")]
        public void Can_ParsePredicateNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
        }

        [TestMethod]
        [DataRow("(pred)", "pred")]
        [DataRow("(pred ?a)", "pred")]
        [DataRow("(pred ?a ?b)", "pred")]
        [DataRow("(q ?a ?b)", "q")]
        public void Can_ParsePredicateNode_CorrectName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
            if (exp is PredicateExp pred)
                Assert.AreEqual(expected, pred.Name);
        }

        [TestMethod]
        [DataRow("(pred ?a)", "a")]
        [DataRow("(pred ?a ?b)", "a", "b")]
        [DataRow("(q ?a ?long)", "a", "long")]
        public void Can_ParsePredicateNode_CorrectParameterNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
            if (exp is PredicateExp pred)
            {
                Assert.AreEqual(expected.Length, pred.Arguments.Count);
                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], pred.Arguments[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(pred ?a)", "")]
        [DataRow("(pred ?a - type)", "type")]
        [DataRow("(pred ?a ?b - type)", "", "type")]
        [DataRow("(pred ?a - type ?b - type)", "type", "type")]
        public void Can_ParsePredicateNode_CorrectParameterTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
            if (exp is PredicateExp pred)
            {
                Assert.AreEqual(expected.Length, pred.Arguments.Count);
                for (int i = 0; i < expected.Length; i++)
                    Assert.AreEqual(expected[i], pred.Arguments[i].Type.Name);
            }
        }

        [TestMethod]
        [DataRow("(name)")]
        [DataRow("(name - type)")]
        public void Can_ParseNameNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
        }

        [TestMethod]
        [DataRow("(name)", "name")]
        [DataRow("(name - type)", "name")]
        [DataRow("(a - type)", "a")]
        public void Can_ParseNameNode_CorrectName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
            if (exp is NameExp name)
                Assert.AreEqual(expected, name.Name);
        }

        [TestMethod]
        [DataRow("(name)", "")]
        [DataRow("(name - type)", "type")]
        [DataRow("(a - type)", "type")]
        public void Can_ParseNameNode_CorrectTypeName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
            if (exp is NameExp name)
                Assert.AreEqual(expected, name.Type.Name);
        }
    }
}
