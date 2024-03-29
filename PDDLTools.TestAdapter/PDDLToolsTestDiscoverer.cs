﻿using Microsoft.VisualStudio.TestPlatform.ObjectModel;
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

            try
            {
                GenerateTestCases(sources, discoverySink);
            }
            catch (Exception ex)
            {
                TestLog.SendErrorMessage($"An error occured while discovering tests: {ex}");
            }
        }

        public static List<TestCase> GenerateTestCases(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            List<TestCase> cases = new List<TestCase>();
            foreach (var file in sources)
                cases.AddRange(GenerateTestCasesFromTestFile(file, discoverySink));
            return cases;
        }

        private static List<TestCase> GenerateTestCasesFromTestFile(string file, ITestCaseDiscoverySink discoverySink)
        {
            List<TestCase> returnList = new List<TestCase>();
            PDDLTest test = JsonConvert.DeserializeObject<PDDLTest>(File.ReadAllText(file));

            var basePath = new FileInfo(file).Directory.FullName;
            var domainPath = Path.Combine(basePath, test.Domain);
            var domainName = new FileInfo(domainPath).Name.Replace(".pddl","");

            // So that the list is prioritized as listed in the .pddltest file
            test.Tasks.Reverse();
            foreach (var problem in test.Problems)
            {
                var problemPath = Path.Combine(basePath, problem);
                var problemName = new FileInfo(problemPath).Name.Replace(".pddl", "");

                foreach(var task in test.Tasks)
                {
                    var newCase = new TestCase($"{domainName}.{problemName}.{task}", new Uri(PDDLToolsTestExecutor.ExecutorUriStr), file.ToLowerInvariant());
                    returnList.Add(newCase);
                    newCase.CodeFilePath = problemPath;
                    newCase.SetPropertyValue(TestProperty.Register("PDDLTools_DomainPath", "DomainPath", typeof(string), typeof(string)), domainPath);
                    newCase.SetPropertyValue(TestProperty.Register("PDDLTools_ProblemPath", "ProblemPath", typeof(string), typeof(string)), problemPath);
                    newCase.SetPropertyValue(TestProperty.Register("PDDLTools_Task", "Task", typeof(string), typeof(string)), task);
                    discoverySink.SendTestCase(newCase);
                }
            }
            return returnList;
        }
    }
}