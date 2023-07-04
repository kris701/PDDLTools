using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Tests.Visitors
{
    [TestClass]
    public class DomainVisitorTests
    {
        [TestMethod]
        [DataRow("(define (domain a))", typeof(DomainDecl))]
        [DataRow("(domain abc)", typeof(DomainNameDecl))]
        [DataRow("(:requirements abc)", typeof(RequirementsDecl))]
        [DataRow("(:extends :abc :other)", typeof(ExtendsDecl))]
        [DataRow("(:predicates (b) (c ?d))", typeof(PredicatesDecl))]
        [DataRow("(:constants a - b)", typeof(ConstantsDecl))]
        [DataRow("(:types a - b \n c - d)", typeof(TypesDecl))]
        [DataRow("(:timeless (a - b))", typeof(TimelessDecl))]
        [DataRow("(:action name :parameters (a) :precondition (not (a)) :effect (a))", typeof(ActionDecl))]
        [DataRow("(:axiom :vars (a) :context (not (a)) :implies (a))", typeof(AxiomDecl))]
        public void Can_VisitGeneral(string toParse, Type expectedType)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            var decl = DomainVisitor.Visit(node, null, null);

            // ASSERT
            Assert.IsInstanceOfType(decl, expectedType);
        }

        [TestMethod]
        [DataRow("(define)")]
        [DataRow("(define (domain a))")]
        public void Can_ParseDomainNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitDomainDeclNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainDecl));
        }

        [TestMethod]
        [DataRow("(domain abc)")]
        [DataRow("(domain)")]
        public void Can_ParseDomainNameNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitDomainNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainNameDecl));
        }

        [TestMethod]
        [DataRow("(:requirements :abc :other)")]
        [DataRow("(:requirements :abc)")]
        [DataRow("(:requirements)")]
        public void Can_ParseRequirementsNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitRequirementsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(RequirementsDecl));
        }

        [TestMethod]
        [DataRow("(:extends :abc :other)")]
        [DataRow("(:extends :abc)")]
        [DataRow("(:extends)")]
        public void Can_ParseExtendsNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitExtendsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ExtendsDecl));
        }

        [TestMethod]
        [DataRow("(:types a - b \n c - d)")]
        [DataRow("(:types a - b)")]
        [DataRow("(:types)")]
        public void Can_ParseTypesNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTypesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
        }

        [TestMethod]
        [DataRow("(:constants)")]
        [DataRow("(:constants a)")]
        [DataRow("(:constants a - b)")]
        public void Can_ParseConstantsNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitConstantsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
        }

        [TestMethod]
        [DataRow("(:predicates)")]
        [DataRow("(:predicates (a))")]
        [DataRow("(:predicates (b) (c ?d))")]
        public void Can_ParsePredicatesNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitPredicatesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(PredicatesDecl));
        }

        [TestMethod]
        [DataRow("(:timeless)")]
        [DataRow("(:timeless (a))")]
        [DataRow("(:timeless (a - b))")]
        public void Can_ParseTimelessNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTimelessNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TimelessDecl));
        }

        [TestMethod]
        [DataRow("(:action name :parameters () :precondition () :effect ())")]
        [DataRow("(:action name :parameters (a) :precondition (not (a)) :effect (a))")]
        public void Can_ParseActionNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitActionNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ActionDecl));
        }

        [TestMethod]
        [DataRow("(:axiom :vars () :context () :implies ())")]
        [DataRow("(:axiom :vars (a) :context (not (a)) :implies (a))")]
        public void Can_ParseAxiomNode(string toParse)
        {
            // ARRANGE
            var parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitAxiomNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(AxiomDecl));
        }
    }
}
