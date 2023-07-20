﻿using CMDRunners;
using CMDRunners.FastDownward;
using CMDRunners.Models;
using Microsoft.ServiceHub.Resources;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Newtonsoft.Json;
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
                var tasks = GenerateTasks(sources, frameworkHandle);

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

        private List<Task> GenerateTasks(IEnumerable<TestCase> sources, IFrameworkHandle frameworkHandle)
        {
            List<Task> tasks = new List<Task>();

            Random rnd = new Random();
            foreach (var source in sources)
            {
                if (_stop)
                    break;

                var split = source.CodeFilePath.Split(';');
                if (split.Length == 2)
                {
                    var domain = split[0];
                    var problem = split[1];
                    if (File.Exists(domain) && File.Exists(problem))
                    {
                        var tempPlanName = $"temp{rnd.Next()}.pddlplan";
                        var tempOutName = $"temp{rnd.Next()}.out";

                        tasks.Add(GenerateTask(source, domain, problem, tempPlanName, tempOutName, frameworkHandle));
                    }
                }
            }

            return tasks;
        }

        private Task GenerateTask(TestCase source, string domain, string problem, string tempPlanName, string tempOutName, IFrameworkHandle frameworkHandle)
        {
            return new Task(() => {
                var outcome = new TestResult(source);
                outcome.StartTime = DateTime.Now;
                outcome.Outcome = TestOutcome.Failed;
                frameworkHandle.RecordStart(source);
                try
                {
                    FDRunner runner = new FDRunner(FastDownwardPath, PythonPrefix, FastDownwardTimeout);
                    var res = runner.RunAsync(domain, problem, FastDownwardEngineArgs, tempPlanName, tempOutName).Result;
                    if (res.ResultReason == CMDRunners.Helpers.ProcessCompleteReson.RanToCompletion && res.ExitCode == FDExitCode.SUCCESS)
                        outcome.Outcome = TestOutcome.Passed;
                    if (res.ResultReason == CMDRunners.Helpers.ProcessCompleteReson.ForceKilled)
                        outcome.Outcome = TestOutcome.Skipped;
                    if (File.Exists(tempPlanName))
                        File.Delete(tempPlanName);
                    if (File.Exists(tempOutName))
                        File.Delete(tempOutName);
                    foreach (var logItem in res.Log)
                    {
                        if (logItem.Type == LogItem.ItemType.Log)
                            outcome.Messages.Add(new TestResultMessage("StdOutMsgs", $"{logItem.Content}{Environment.NewLine}"));
                        else if (logItem.Type == LogItem.ItemType.Error)
                            outcome.Messages.Add(new TestResultMessage("StdErrMsgs", $"{logItem.Content}{Environment.NewLine}"));
                    }
                }
                catch (Exception ex)
                {
                    outcome.ErrorMessage = ex.Message;
                    outcome.Messages.Add(new TestResultMessage("StdErrMsgs", ex.Message));
                }
                outcome.EndTime = DateTime.Now;
                outcome.Duration = outcome.EndTime - outcome.StartTime;
                frameworkHandle.RecordResult(outcome);
            });
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
