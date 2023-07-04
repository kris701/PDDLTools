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
            var parser = new ASTParser();
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
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitAndNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(AndExp));
        }

        [TestMethod]
        [DataRow("(or () ())")]
        [DataRow("(or (abcas) (qwefas))")]
        public void Can_ParseOrNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitOrNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(OrExp));
        }

        [TestMethod]
        [DataRow("(not ())")]
        [DataRow("(not (abcas))")]
        public void Can_ParseNotNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNotNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NotExp));
        }

        [TestMethod]
        [DataRow("(pred)")]
        [DataRow("(pred ?a)")]
        [DataRow("(pred ?a ?b)")]
        public void Can_ParsePredicateNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitPredicateNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(PredicateExp));
        }

        [TestMethod]
        [DataRow("(name)")]
        [DataRow("(name=t=type)")]
        public void Can_ParseNameNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IExp exp;
            ExpVisitor.TryVisitNameNode(node, null, null, out exp);

            // ASSERT
            Assert.IsInstanceOfType(exp, typeof(NameExp));
        }
    }
}
