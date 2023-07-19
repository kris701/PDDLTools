using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PDDLTools.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PDDLTools.TestAdapter
{
    [FileExtension(".pddltest")]
    [DefaultExecutorUri(PDDLToolsTestExecutor.ExecutorUriStr)]
    public sealed class PDDLToolsTestDiscoverer : PDDLTestAdapter, ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger messageLogger, ITestCaseDiscoverySink discoverySink)
        {
            TestLog.Initialize(messageLogger);
            Initialize(discoveryContext);
            TestLog.SendInformationalMessage("Discovering PDDL test files...");
            int counter = 1;
            foreach (var file in sources)
            {
                TestLog.SendInformationalMessage($"Generating test case {counter++} out of {sources.Count()}");
                try
                {
                    GenerateTestCasesFromTestFile(file, discoverySink);
                }
                catch (Exception ex)
                {
                    TestLog.SendErrorMessage($"An error occured while discovering tests: {ex}");
                }
            }
        }

        private void GenerateTestCasesFromTestFile(string file, ITestCaseDiscoverySink discoverySink)
        {
            PDDLTest test = JsonConvert.DeserializeObject<PDDLTest>(File.ReadAllText(file));
            var basePath = new FileInfo(test.Domain).Name;
            foreach(var problem in test.Problems)
            {
                var caseName = Path.Combine(basePath, new FileInfo(problem).Name.Replace(".pddl", ""));
                var newCase = new TestCase(caseName, new Uri(PDDLToolsTestExecutor.ExecutorUriStr), file.ToLowerInvariant());
                newCase.CodeFilePath = problem;
                discoverySink.SendTestCase(newCase);
            }
        }
    }
}