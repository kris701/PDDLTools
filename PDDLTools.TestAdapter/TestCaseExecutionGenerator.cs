using CMDRunners.FastDownward;
using CMDRunners.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using PDDLParser.Exceptions;
using PDDLParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PDDLTools.TestAdapter
{
    public class TestCaseExecutionGenerator : PDDLTestAdapter
    {
        public readonly static HashSet<string> TaskOptions = new HashSet<string>() { "Parse", "ParseAnalyse", "FDExecute" };

        public TestCaseExecutionGenerator(IDiscoveryContext context) {
            Initialize(context);
        }

        public bool IsTaskValid(string task) => TaskOptions.Any(x => x.ToLower() == task.ToLower());

        public Task GetTask(TestCase source, string domain, string problem, string task, IFrameworkHandle frameworkHandle)
        {
            switch (task.ToLower())
            {
                case "parse":
                    return GenerateParseTask(source, domain, problem, false, frameworkHandle);
                case "parseanalyse":
                    return GenerateParseTask(source, domain, problem, true, frameworkHandle);
                case "fdexecute":
                    Random rnd = new Random();
                    var tempPlanName = $"temp{rnd.Next()}.pddlplan";
                    var tempOutName = $"temp{rnd.Next()}.out";
                    return GenerateFDExecuteTask(
                        source,
                        domain,
                        problem,
                        tempPlanName,
                        tempOutName,
                        frameworkHandle);
                default:
                    return null;
            }
        }

        private Task GenerateParseTask(TestCase source, string domain, string problem, bool analyse, IFrameworkHandle frameworkHandle)
        {
            return new Task(() => {
                var outcome = new TestResult(source);
                outcome.StartTime = DateTime.Now;
                outcome.Outcome = TestOutcome.Failed;
                frameworkHandle.RecordStart(source);

                try
                {
                    IPDDLParser parser = new PDDLParser.PDDLParser(analyse, analyse);
                    parser.Parse(domain, problem);
                    if (parser.Listener.Errors.Count(x => x.Type == PDDLParser.Listener.ParseErrorType.Error) == 0)
                    {
                        outcome.Outcome = TestOutcome.Passed;
                        foreach (var message in parser.Listener.Errors)
                            outcome.Messages.Add(new TestResultMessage("StdOutMsgs", $"{message}{Environment.NewLine}"));
                    }

                }
                catch (ParseException ex)
                {
                    outcome.ErrorMessage = ex.Message;
                    foreach (var error in ex.Errors)
                        outcome.Messages.Add(new TestResultMessage("StdErrMsgs", $"{error}{Environment.NewLine}"));
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

        private Task GenerateFDExecuteTask(TestCase source, string domain, string problem, string tempPlanName, string tempOutName, IFrameworkHandle frameworkHandle)
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
    }
}
