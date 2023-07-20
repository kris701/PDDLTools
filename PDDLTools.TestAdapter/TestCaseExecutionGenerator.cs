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
    public static class TestCaseExecutionGenerator
    {
        public static Task GenerateParseTask(TestCase source, string domain, string problem, bool analyse, IFrameworkHandle frameworkHandle)
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
                            outcome.Messages.Add(new TestResultMessage("StdOutMsgs", $"{message.Message}{Environment.NewLine}"));
                    }

                }
                catch (ParseException ex)
                {
                    outcome.ErrorMessage = ex.Message;
                    foreach (var error in ex.Errors)
                        outcome.Messages.Add(new TestResultMessage("StdErrMsgs", $"{error.Message}{Environment.NewLine}"));
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

        public static Task GenerateFDExecuteTask(TestCase source, string domain, string problem, string tempPlanName, string tempOutName, string fastDownwardPath, string pythonPrefix, int fastDownwardTimeout, string fastDownwardEngineArgs, IFrameworkHandle frameworkHandle)
        {
            return new Task(() => {
                var outcome = new TestResult(source);
                outcome.StartTime = DateTime.Now;
                outcome.Outcome = TestOutcome.Failed;
                frameworkHandle.RecordStart(source);
                try
                {
                    FDRunner runner = new FDRunner(fastDownwardPath, pythonPrefix, fastDownwardTimeout);
                    var res = runner.RunAsync(domain, problem, fastDownwardEngineArgs, tempPlanName, tempOutName).Result;
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
