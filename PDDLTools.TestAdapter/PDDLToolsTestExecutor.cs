using CMDRunners;
using CMDRunners.FastDownward;
using CMDRunners.Models;
using Microsoft.ServiceHub.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
using PDDLParser;
using PDDLParser.Exceptions;
using PDDLTools.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDDLTools.TestAdapter
{
    [ExtensionUri(ExecutorUriStr)]
    public sealed class PDDLToolsTestExecutor : PDDLTestAdapter, ITestExecutor
    {
        public const string ExecutorUriStr = "executor://PDDLToolsTestExecutor/v1";

        private bool _stop = false;

        public void Cancel()
        {
            _stop = true;
        }

        public void RunTests(IEnumerable<TestCase> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            Initialize(runContext);
            TestLog.Initialize(frameworkHandle);
            _stop = false;

            TestLog.SendInformationalMessage($"Verifying runsettings");
            var validateParser = new FDRunner(FastDownwardPath, PythonPrefix, FastDownwardTimeout);
            if (!validateParser.VerifyExecutable())
            {
                TestLog.SendErrorMessage($"Runsettings are invalid! Make sure the paths and names for all items are correct.");
                TestLog.SendErrorMessage($"Runsettings used: {runContext.RunSettings.SettingsXml}");
            }
            else
            {
                var tasks = GenerateTasks(sources, runContext, frameworkHandle);

                if (RunParallel)
                {
                    TestLog.SendInformationalMessage($"Executing all {tasks.Count} tests in parallel.");
                    foreach (var task in tasks)
                        task.Start();
                    Task.WhenAll(tasks).Wait();
                }
                else
                {
                    int counter = 0;
                    foreach (var task in tasks)
                    {
                        TestLog.SendInformationalMessage($"Executing {counter++} out of {tasks.Count}");
                        if (_stop)
                            break;
                        task.Start();
                        task.Wait();
                    }
                }
                _stop = false;
            }
        }

        private List<Task> GenerateTasks(IEnumerable<TestCase> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            List<Task> tasks = new List<Task>();

            var generator = new TestCaseExecutionGenerator(runContext);

            foreach (var source in sources)
            {
                if (_stop)
                    break;

                var domain = source.GetPropertyValue<string>(TestProperty.Find("PDDLTools_DomainPath"), "None");
                var problem = source.GetPropertyValue<string>(TestProperty.Find("PDDLTools_ProblemPath"), "None");
                var task = source.GetPropertyValue<string>(TestProperty.Find("PDDLTools_Task"), "");
                if (File.Exists(domain) && File.Exists(problem))
                {
                    if (generator.IsTaskValid(task))
                    {
                        var newTask = generator.GetTask(source, domain, problem, task, frameworkHandle);
                        if (newTask != null)
                            tasks.Add(newTask);
                        else
                            TestLog.SendErrorMessage($"Unknown error occured while processing the domain '{domain}' and '{problem}' case! The task was: {task}");
                    }
                    else
                        TestLog.SendErrorMessage($"Invalid task given for the domain '{domain}' and '{problem}' case! The task was: {task}");
                }
                else
                    TestLog.SendErrorMessage($"Could not find domain and problem file! Domain: '{domain}', Problem: '{problem}'");
            }

            return tasks;
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            TestLog.Initialize(frameworkHandle);
            try
            {
                TestLog.SendInformationalMessage("Running from process:" + Process.GetCurrentProcess() + " ID:" + Process.GetCurrentProcess().Id.ToString());
                foreach (var source in sources)
                {
                    TestLog.SendInformationalMessage("Finding tests in source:" + source);
                }

                IEnumerable<TestCase> tests = PDDLToolsTestDiscoverer.GenerateTestCases(sources, null);
                foreach (var test in tests)
                {
                    TestLog.SendInformationalMessage("Found test:" + test.DisplayName);
                }
                RunTests(tests, runContext, frameworkHandle);
            }
            catch (Exception e)
            {
                TestLog.SendErrorMessage("Exception during test execution: " + e.Message);
            }
        }
    }
}
