using SASSimulator.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator.UnitTests.Parsers
{
    [TestClass]
    public class PlanParserTests
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("(a)", "a")]
        [DataRow("(a)\n(b)", "a", "b")]
        [DataRow("(a)\n(b)\n;(c)", "a", "b")]
        [DataRow("(a)\n;(b)\n(c)", "a", "c")]
        public void Can_ParsePlanFile(string content, params string[] expectedNames)
        {
            // ARRANGE
            if (File.Exists("empty.pddlplan"))
                File.Delete("empty.pddlplan");
            File.WriteAllText("empty.pddlplan", content);
            IPlanParser parser = new PlanParser();

            // ACT
            var result = parser.ParsePlanFile("empty.pddlplan");

            // ASSERT
            Assert.AreEqual(expectedNames.Length, result.Count);
            for (int i = 0; i < expectedNames.Length; i++)
                Assert.AreEqual(expectedNames[i], result[i].Name);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("(a)", "a")]
        [DataRow("(a)\n(b)", "a", "b")]
        [DataRow("(a)\n(b)\n;(c)", "a", "b")]
        [DataRow("(a)\n;(b)\n(c)", "a", "c")]
        public void Can_ParsePlanText_1(string content, params string[] expectedNames)
        {
            // ARRANGE
            IPlanParser parser = new PlanParser();

            // ACT
            var result = parser.ParsePlanText(content);

            // ASSERT
            Assert.AreEqual(expectedNames.Length, result.Count);
            for (int i = 0; i < expectedNames.Length; i++)
                Assert.AreEqual(expectedNames[i], result[i].Name);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("(a)", "a")]
        [DataRow("(a)\n(b)", "a", "b")]
        [DataRow("(a)\n(b)\n;(c)", "a", "b")]
        [DataRow("(a)\n;(b)\n(c)", "a", "c")]
        public void Can_ParsePlanText_2(string content, params string[] expectedNames)
        {
            // ARRANGE
            IPlanParser parser = new PlanParser();

            // ACT
            var result = parser.ParsePlanText(content.Split('\n'));

            // ASSERT
            Assert.AreEqual(expectedNames.Length, result.Count);
            for (int i = 0; i < expectedNames.Length; i++)
                Assert.AreEqual(expectedNames[i], result[i].Name);
        }
    }
}
