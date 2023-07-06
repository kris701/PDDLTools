using PDDLParser.AST;
using PDDLParser.Listener;
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
            IASTParser<ASTNode> parser = new ASTParser();
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
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitDomainDeclNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainDecl));
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
            DomainVisitor.TryVisitDomainDeclNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.StrayCharactersFound);
        }

        [TestMethod]
        [DataRow("(domain abc)")]
        [DataRow("(domain   abc)")]
        [DataRow("(domain   aasdafabc)")]
        public void Can_ParseDomainNameNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitDomainNameNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(DomainNameDecl));
        }

        [TestMethod]
        [DataRow("(domain )")]
        [DataRow("(domain     )")]
        public void Cantt_VisitProblemDecl_IfContainsNoName(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitDomainNameNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactLooseChildren);
        }

        [TestMethod]
        [DataRow("(domain abc)")]
        public void Can_ParseDomainNameNode_CorrectName(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
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
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitRequirementsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(RequirementsDecl));
        }

        [TestMethod]
        [DataRow("(:requirements :abc :other)", ":abc", ":other")]
        [DataRow("(:requirements :abc)", ":abc")]
        public void Can_ParseRequirementsNode_CorrectNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitRequirementsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(RequirementsDecl));
            if (decl is RequirementsDecl reqs)
            {
                Assert.AreEqual(expected.Length, reqs.Requirements.Count);
                for (int i = 0; i < reqs.Requirements.Count; i++)
                    Assert.AreEqual(expected[i], reqs.Requirements[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:extends :abc :other)")]
        [DataRow("(:extends :abc)")]
        [DataRow("(:extends)")]
        public void Can_ParseExtendsNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitExtendsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ExtendsDecl));
        }

        [TestMethod]
        [DataRow("(:extends :abc :other)", ":abc", ":other")]
        [DataRow("(:extends :abc)", ":abc")]
        public void Can_ParseExtendsNode_CorrectNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitExtendsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ExtendsDecl));
            if (decl is ExtendsDecl exts)
            {
                Assert.AreEqual(expected.Length, exts.Extends.Count);
                for (int i = 0; i < exts.Extends.Count; i++)
                    Assert.AreEqual(expected[i], exts.Extends[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:types a - b \n c - d)")]
        [DataRow("(:types a - b)")]
        [DataRow("(:types)")]
        public void Can_ParseTypesNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTypesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
        }

        [TestMethod]
        [DataRow("(:types a - b \n c - d)", "b", "d")]
        [DataRow("(:types a q - b \n e c - d)", "b", "d")]
        [DataRow("(:types a - b)", "b")]
        public void Can_ParseTypesNode_CorrectSuperTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTypesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
            if (decl is TypesDecl types)
            {
                Assert.AreEqual(expected.Length, types.Types.Count);
                for (int i = 0; i < types.Types.Count; i++)
                    Assert.AreEqual(expected[i], types.Types[i].TypeName.Name);
            }
        }

        [TestMethod]
        [DataRow("(:types a - b \n c - d)", "a", "c")]
        [DataRow("(:types a q - b \n e c - d)", "a", "q", "e", "c")]
        [DataRow("(:types a - b)", "a")]
        public void Can_ParseTypesNode_CorrectSubTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTypesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TypesDecl));
            if (decl is TypesDecl types)
            {
                int offset = 0;
                for (int i = 0; i < types.Types.Count; i++)
                    for (int j = 0; j < types.Types[i].SubTypes.Count; j++)
                        Assert.AreEqual(expected[offset++], types.Types[i].SubTypes[j].Name);
                Assert.AreEqual(expected.Length, offset);
            }
        }

        [TestMethod]
        [DataRow("(:constants)")]
        [DataRow("(:constants a)")]
        [DataRow("(:constants a b)")]
        [DataRow("(:constants a - b)")]
        [DataRow("(:constants a - b other - type)")]
        public void Can_ParseConstantsNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitConstantsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
        }

        [TestMethod]
        [DataRow("(:constants a)", "a")]
        [DataRow("(:constants a b)", "a", "b")]
        [DataRow("(:constants a - b)", "a")]
        [DataRow("(:constants a - b other - type)", "a", "other")]
        public void Can_ParseConstantsNode_CorrectNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitConstantsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
            if (decl is ConstantsDecl con)
            {
                Assert.AreEqual(expected.Length, con.Constants.Count);
                for (int i = 0; i < con.Constants.Count; i++)
                    Assert.AreEqual(expected[i], con.Constants[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:constants a - b)", "b")]
        [DataRow("(:constants a - b other - type)", "b", "type")]
        [DataRow("(:constants a - b other - type c)", "b", "type", "")]
        public void Can_ParseConstantsNode_CorrectTypeNames(string toParse, params string[] expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitConstantsNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ConstantsDecl));
            if (decl is ConstantsDecl con)
            {
                Assert.AreEqual(expected.Length, con.Constants.Count);
                for (int i = 0; i < con.Constants.Count; i++)
                    Assert.AreEqual(expected[i], con.Constants[i].Type.Name);
            }
        }

        [TestMethod]
        [DataRow("(:predicates)")]
        [DataRow("(:predicates (a))")]
        [DataRow("(:predicates (b) (c ?d))")]
        public void Can_ParsePredicatesNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitPredicatesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(PredicatesDecl));
        }

        [TestMethod]
        [DataRow("(:predicates (a))", "a")]
        [DataRow("(:predicates (b) (c ?d))", "b", "c")]
        public void Can_ParsePredicatesNode_CorrectName(string toParse, params string[] expecteds)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitPredicatesNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(PredicatesDecl));
            if (decl is PredicatesDecl preds)
            {
                Assert.AreEqual(preds.Predicates.Count, expecteds.Length);
                for (int i = 0; i < preds.Predicates.Count; i++)
                    Assert.AreEqual(expecteds[i], preds.Predicates[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:timeless)")]
        [DataRow("(:timeless (a))")]
        [DataRow("(:timeless (a - b))")]
        [DataRow("(:timeless (a - b) (c))")]
        public void Can_ParseTimelessNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTimelessNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TimelessDecl));
        }

        [TestMethod]
        [DataRow("(:timeless (a))", "a")]
        [DataRow("(:timeless (a - b))", "a")]
        [DataRow("(:timeless (a - b) (c))", "a", "c")]
        [DataRow("(:timeless (a - b) (c)    (d))", "a", "c", "d")]
        public void Can_ParseTimelessNode_CorrectNames(string toParse, params string[] expecteds)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitTimelessNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(TimelessDecl));
            if (decl is TimelessDecl timel)
            {
                Assert.AreEqual(expecteds.Length, timel.Items.Count);
                for (int i = 0; i < timel.Items.Count; i++)
                    Assert.AreEqual(expecteds[i], timel.Items[i].Name);
            }
        }

        [TestMethod]
        [DataRow("(:action name :parameters () :precondition () :effect ())")]
        [DataRow("(:action name :parameters (a) :precondition (not (a)) :effect (a))")]
        public void Can_ParseActionNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitActionNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ActionDecl));
        }

        [TestMethod]
        [DataRow("(:action name :parameters () :precondition () :effect ())", "name")]
        [DataRow("(:action othername :parameters (a) :precondition (not (a)) :effect (a))", "othername")]
        public void Can_ParseActionNode_CorrectActionName(string toParse, string expected)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitActionNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(ActionDecl));
            if (decl is ActionDecl act)
                Assert.AreEqual(expected, act.Name);
        }

        [TestMethod]
        [DataRow("(:action name () :precondition () :effect ())")]
        [DataRow("(:action name :parameters () () :effect ())")]
        [DataRow("(:action name :parameters () :precondition () ())")]
        [DataRow("(:action name () () ())")]

        public void Cant_ParseActionNode_IfMissingPrimaryElements(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitActionNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.UnexpectedNodeType);
        }

        [TestMethod]
        [DataRow("(:action  :parameters () :precondition () :effect ())")]
        [DataRow("(:action        :parameters () :precondition () :effect ())")]

        public void Cant_ParseActionNode_IfMissingName(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitActionNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.NeedExactLooseChildren);
        }

        [TestMethod]
        [DataRow("(:axiom :vars () :context () :implies ())")]
        [DataRow("(:axiom :vars (a) :context (not (a)) :implies (a))")]
        public void Can_ParseAxiomNode(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitAxiomNode(node, null, null, out decl);

            // ASSERT
            Assert.IsInstanceOfType(decl, typeof(AxiomDecl));
        }

        [TestMethod]
        [DataRow("(:axiom () :context () :implies ())")]
        [DataRow("(:axiom :vars () () :implies ())")]
        [DataRow("(:axiom :vars () :context () ())")]
        [DataRow("(:axiom () () ())")]
        public void Cant_ParseAxiomNode_IfMissingPrimaryElements(string toParse)
        {
            // ARRANGE
            IASTParser<ASTNode> parser = new ASTParser();
            var node = parser.Parse(toParse);
            IErrorListener listener = new ErrorListener();
            listener.ThrowIfTypeAbove = ParseErrorType.Error;

            // ACT
            IDecl decl;
            DomainVisitor.TryVisitAxiomNode(node, null, listener, out decl);

            // ASSERT
            Assert.IsTrue(listener.Errors.Count > 0);
            Assert.IsTrue(listener.Errors[0].Code == ParserErrorCode.UnexpectedNodeType);
        }
    }
}
