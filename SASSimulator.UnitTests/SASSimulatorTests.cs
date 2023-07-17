using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using SASSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator.UnitTests
{
    [TestClass]
    public class SASSimulatorTests
    {
        #region Constructor
        [TestMethod]
        public void Can_SetInitialState_1()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Problem.Init = new InitDecl(
                new ASTNode(),
                null,
                new List<IExp>()
                {
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                    })
                });

            // ACT
            ISASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>());

            // ASSERT
            Assert.AreEqual(decl.Problem.Init.Predicates.Count, simulator.State.Count);
            var item = simulator.State.Single(x => x.Name == "pred-1");
            Assert.AreEqual("pred-1", item.Name);
            Assert.AreEqual("?a", item.Arguments[0]);
            Assert.AreEqual("?b", item.Arguments[1]);
        }

        [TestMethod]
        public void Can_SetInitialState_2()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Problem.Init = new InitDecl(
                new ASTNode(),
                null,
                new List<IExp>()
                {
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                    }),
                    new PredicateExp(new ASTNode(), null, "pred-2", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a")
                    })
                });

            // ACT
            ISASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>());

            // ASSERT
            Assert.AreEqual(decl.Problem.Init.Predicates.Count, simulator.State.Count);
            var item = simulator.State.Single(x => x.Name == "pred-1");
            Assert.AreEqual("pred-1", item.Name);
            Assert.AreEqual("?a", item.Arguments[0]);
            Assert.AreEqual("?b", item.Arguments[1]);
            var item2 = simulator.State.Single(x => x.Name == "pred-2");
            Assert.AreEqual("pred-2", item2.Name);
            Assert.AreEqual("?a", item2.Arguments[0]);
        }
        #endregion

        #region Step

        [TestMethod]
        public void Can_Step_1()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                                new NameExp(new ASTNode(), null, "?a"),
                                new NameExp(new ASTNode(), null, "?b")
                }),
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                    }))
                ));
            decl.Problem.Init = new InitDecl(
                new ASTNode(),
                null,
                new List<IExp>()
                {
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                    })
                });
            ISASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>() {
                new ActionChoice("test-act", new List<string>(){ "?a", "?b" })
            });

            // ACT
            Assert.AreEqual(1, simulator.State.Count);
            simulator.Step();
            Assert.AreEqual(0, simulator.State.Count);
        }

        [TestMethod]
        public void Can_Step_2()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                                new NameExp(new ASTNode(), null, "?a"),
                                new NameExp(new ASTNode(), null, "?b")
                }),
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                    }))
                ));
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act2",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                                new NameExp(new ASTNode(), null, "?a"),
                                new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                    }))
                ,
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                })));
            decl.Problem.Init = new InitDecl(
                new ASTNode(),
                null,
                new List<IExp>()
                {
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                    })
                });
            ISASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>() {
                new ActionChoice("test-act", new List<string>(){ "?a", "?b" }),
                new ActionChoice("test-act2", new List<string>(){ "?a", "?b" }),
            });

            // ACT
            Assert.AreEqual(1, simulator.State.Count);
            simulator.Step();
            Assert.AreEqual(0, simulator.State.Count);
            simulator.Step();
            Assert.AreEqual(1, simulator.State.Count);
        }

        [TestMethod]
        public void Can_Step_3()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                                new NameExp(new ASTNode(), null, "?a"),
                                new NameExp(new ASTNode(), null, "?b")
                }),
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                    }))
                ));
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act2",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                                new NameExp(new ASTNode(), null, "?a"),
                                new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                    }))
                ,
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                                    new NameExp(new ASTNode(), null, "?a"),
                                    new NameExp(new ASTNode(), null, "?b")
                })));
            decl.Problem.Init = new InitDecl(
                new ASTNode(),
                null,
                new List<IExp>()
                {
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "obja"),
                        new NameExp(new ASTNode(), null, "objb")
                    })
                });
            ISASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>() {
                new ActionChoice("test-act", new List<string>(){ "obja", "objb" }),
                new ActionChoice("test-act2", new List<string>(){ "obja", "objb" }),
            });

            // ACT
            Assert.AreEqual(1, simulator.State.Count);
            simulator.Step();
            Assert.AreEqual(0, simulator.State.Count);
            simulator.Step();
            Assert.AreEqual(1, simulator.State.Count);
        }

        #endregion

        #region FindAction

        [TestMethod]
        public void Can_FindAction()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                    new NameExp(new ASTNode(), null, "?a"),
                    new NameExp(new ASTNode(), null, "?b")
                }),
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                    }))
                ));
            SASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>());

            // ACT
            var act = simulator.FindAction("test-act");

            // ASSERT
            Assert.AreEqual(decl.Domain.Actions[0], act);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Cant_FindAction_IfNotThere()
        {
            // ARRANGE
            PDDLDecl decl = new PDDLDecl(
                new DomainDecl(new ASTNode()),
                new ProblemDecl(new ASTNode())
                );
            decl.Domain.Actions.Add(new ActionDecl(
                new ASTNode(),
                decl.Domain,
                "test-act",
                new ParameterDecl(new ASTNode(), null, new List<NameExp>() {
                    new NameExp(new ASTNode(), null, "?a"),
                    new NameExp(new ASTNode(), null, "?b")
                }),
                new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                }),
                new NotExp(new ASTNode(), null,
                    new PredicateExp(new ASTNode(), null, "pred-1", new List<NameExp>(){
                        new NameExp(new ASTNode(), null, "?a"),
                        new NameExp(new ASTNode(), null, "?b")
                    }))
                ));
            SASSimulator simulator = new SASSimulator(decl, new List<ActionChoice>());

            // ACT
            var act = simulator.FindAction("test-act1");
        }

        #endregion
    }
}
