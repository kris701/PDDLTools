using PDDLParser.AST;
using PDDLParser.Models;
using PDDLParser.UnitTests.PositionTestsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.UnitTests.Visitors
{
    [TestClass]
    public class PositionTests
    {
        [TestMethod]
        [DataRow("PositionTestsData/gripper-domain.pddl", "PositionTestsData/gripper-domain-expected.txt")]
        [DataRow("PositionTestsData/construction-domain.pddl", "PositionTestsData/construction-domain-expected.txt")]
        public void Can_ASTParser_SetCorrectPossitions(string testFile, string expectedFile)
        {
            // ARRANGE
            var expectedNode = PositionNode.ParseExpectedFile(expectedFile);
            IPDDLParser pddlParser = new PDDLParser(false, false);

            // ACT
            var node = pddlParser.Parse(testFile);

            // ASSERT
            IsNodePositionValid(node.Domain, expectedNode);
        }

        private void IsNodePositionValid(INode node, PositionNode expectedNode)
        {
            Assert.AreEqual(expectedNode.Start, node.Start, $"Start did not match in node '{expectedNode.NodeType}'");
            Assert.AreEqual(expectedNode.End, node.End, $"End did not match in node '{expectedNode.NodeType}'");
            Assert.AreEqual(expectedNode.NodeType, node.GetType().Name, $"Type did not match in node '{expectedNode.NodeType}'");

            if (node is IWalkable walkable)
            {
                int index = 0;
                foreach (var child in walkable)
                {
                    //if (index >= expectedNode.Children.Count)
                    //    Assert.Fail($"Node did not have the expected number of children! Node {expectedNode.NodeType}, expected {expectedNode.Children.Count} children");
                    
                    IsNodePositionValid(child, expectedNode.Children[index++]);
                }
            }
        }
    }
}
