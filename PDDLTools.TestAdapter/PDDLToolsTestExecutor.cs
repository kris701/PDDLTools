using CMDRunners;
using CMDRunners.FastDownward;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
using PDDLTools.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            TestLog.Initialize(frameworkHandle);
            _stop = false;
            foreach (var source in sources)
            {
                if (_stop)
                    break;

                TestLog.SendInformationalMessage($"Executing: {source.DisplayName}");
                frameworkHandle.RecordStart(source);
                var outcome = new TestResult(source);
                outcome.Outcome = TestOutcome.Failed;
                try
                {
                    var domain = "";
                    var problem = "";
                    if (source.LocalExtensionData is KeyValuePair<string, string> kv)
                    {
                        domain = kv.Key;
                        problem = kv.Value;
                    }
                    TestLog.SendInformationalMessage($"   Full Domain:  {domain}");
                    TestLog.SendInformationalMessage($"   Full Problem: {problem}");
                    if (!File.Exists(domain) || !File.Exists(problem)) {
                        outcome.Outcome = TestOutcome.NotFound;
                    }
                    else 
                    {
                        FDRunner runner = new FDRunner(FastDownwardPath, PythonPrefix, FastDownwardTimeout);
                        var res = runner.RunAsync(domain, problem, FastDownwardEngineArgs, "temp.pddlplan", "temp.sas").Result;
                        if (res.ExitCode == FDExitCode.SUCCESS)
                            outcome.Outcome = TestOutcome.Passed;
                        if (File.Exists("temp.pddlplan"))
                            File.Delete("temp.pddlplan");
                        if (File.Exists("temp.sas"))
                            File.Delete("temp.sas");
                    }
                }
                catch (Exception ex)
                {
                    TestLog.SendErrorMessage($"An error occured while executing tests: {ex}");
                }
                TestLog.SendInformationalMessage($"Finished '{source.DisplayName}' with result '{outcome.Outcome}'");
                frameworkHandle.RecordResult(outcome);
            }
            _stop = false;
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
