namespace PDDLTools.Projects
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using CMDRunners;
    using CMDRunners.FastDownward;
    using CMDRunners.Helpers;
    using CMDRunners.Models;
    using Microsoft.VisualStudio.ProjectSystem;
    using Microsoft.VisualStudio.ProjectSystem.Debug;
    using Microsoft.VisualStudio.ProjectSystem.Properties;
    using Microsoft.VisualStudio.ProjectSystem.VS.Debug;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using PDDLParser.Helpers;
    using PDDLTools.Commands;
    using PDDLTools.Helpers;
    using PDDLTools.Options;
    using PDDLTools.Windows.FDResultsWindow;
    using PDDLTools.Windows.SASSolutionWindow;
    using Microsoft.VisualStudio.Threading;
    using static Microsoft.VisualStudio.VSConstants;

    [ExportDebugger("PDDLExecuter")]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    public class PDDLExecuterDebuggerLaunchProvider : DebugLaunchProviderBase
    {
        //[Import]
        internal PDDLConfiguredProject Project { get; set; }
        internal Guid DebuggerID { get; }

        private static OutputPanelController OutputPanel = new OutputPanelController("Fast Downward Output");
        private DateTime _lastRefresh = DateTime.Now;
        private string _lastDomain = "";
        private string _lastProblem = "";
        private string _lastEngine = "";
        private bool _lastCheckResult = false;

        [ImportingConstructor]
        public PDDLExecuterDebuggerLaunchProvider(PDDLConfiguredProject configuredProject)
            : base(configuredProject.ConfiguredProject)
        {
            Project = configuredProject;
            DebuggerID = Guid.NewGuid();
        }

        [ExportPropertyXamlRuleDefinition("PDDL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9be6e469bc4921f1", "XamlRuleToCode:PDDLExecuter.xaml", "Project")]
        [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
        private object DebuggerXaml { get { throw new NotImplementedException(); } }

        private async Task LoadFromSavedProjectPropertiesAsync(PDDLConfiguredProject proj)
        {
            _lastDomain = await proj.GetSelectedDomainAsync();
            _lastProblem = await proj.GetSelectedProblemAsync();
            _lastEngine = await proj.GetSelectedEngineAsync();
        }

        public override async Task<bool> CanLaunchAsync(DebugLaunchOptions launchOptions)
        {
            if (OptionsManager.Instance == null)
                return false;
            if (Project != null && (DateTime.Now - _lastRefresh).Seconds > 1)
            {
                await LoadFromSavedProjectPropertiesAsync(Project);
                _lastCheckResult = PDDLHelper.IsFileDomain(_lastDomain) && PDDLHelper.IsFileProblem(_lastProblem) && _lastEngine != "";
                _lastRefresh = DateTime.Now;
            }
            return _lastCheckResult;
        }

        public override Task<IReadOnlyList<IDebugLaunchSettings>> QueryDebugTargetsAsync(DebugLaunchOptions launchOptions)
        {
            var settings = new DebugLaunchSettings(launchOptions);
            return Task.FromResult(new IDebugLaunchSettings[] { settings } as IReadOnlyList<IDebugLaunchSettings>);
        }

        public override async Task LaunchAsync(DebugLaunchOptions launchOptions)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ThreadingService.JoinableTaskFactory.RunAsync(async delegate
            {
                await ThreadingService.JoinableTaskFactory.SwitchToMainThreadAsync();
                await OutputPanel.InitializeAsync();
                await OutputPanel.ClearOutputAsync();

                bool canLaunch = false;
                if (OptionsManager.Instance != null)
                {
                    if (OptionsManager.Instance.ParseCheckBeforeExecute)
                    {
                        await OutputPanel.WriteLineAsync("Checking if files are valid...");
                        await OutputPanel.WriteLineAsync($"Domain: {_lastDomain}");
                        await OutputPanel.WriteLineAsync($"Problem: {_lastProblem}");
                        PDDLParser.IPDDLParser parser = new PDDLParser.PDDLParser();
                        try
                        {
                            parser.Parse(_lastDomain, _lastProblem);
                            await OutputPanel.WriteLineAsync("Files are valid!");
                            canLaunch = true;
                        }
                        catch
                        {
                            await OutputPanel.WriteLineAsync("There are parse errors in the execution files!");
                            await OutputPanel.WriteLineAsync("(You can disable this check in the settings)");
                        }
                        await OutputPanel.WriteLineAsync(
                            $"Errors: {parser.Listener.Errors.Count(x => x.Type == PDDLParser.Listener.ParseErrorType.Error)}, " +
                            $"Warnings: {parser.Listener.Errors.Count(x => x.Type == PDDLParser.Listener.ParseErrorType.Warning)}, " +
                            $"Messages: {parser.Listener.Errors.Count(x => x.Type == PDDLParser.Listener.ParseErrorType.Message)}, ");
                    }
                    else
                        canLaunch = true;
                }
                else
                    canLaunch = true;

                if (canLaunch)
                {
                    var proj = await PDDLProjectManager.GetCurrentProjectAsync();
                    var dir = await proj.GetProjectPathAsync();
                    var outPath = Path.Combine(dir, OptionsManager.Instance.OutputPlanPath);
                    var intPath = Path.Combine(dir, OptionsManager.Instance.IntermediateOutputPath);
                    if (!Directory.Exists(outPath))
                        Directory.CreateDirectory(outPath);
                    if (!Directory.Exists(intPath))
                        Directory.CreateDirectory(intPath);

                    var planName = $"{new FileInfo(_lastDomain).Name.Replace(".pddl", "")}-{new FileInfo(_lastProblem).Name.Replace(".pddl", "")}";

                    await OutputPanel.WriteLineAsync("Executing PDDL File");

                    await TaskScheduler.Default;
                    var resultData = await ExecuteFDAsync(outPath, planName, intPath);
                    await ThreadingService.JoinableTaskFactory.SwitchToMainThreadAsync();

                    await WriteToOutputWindowAsync(resultData);
                    if (resultData.ResultReason == ProcessCompleteReson.RanToCompletion)
                        await SetupResultWindowsAsync(resultData, _lastDomain, _lastProblem, Path.Combine(outPath, $"{planName}.pddlplan"));
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        private async Task<FDResults> ExecuteFDAsync(string outPath, string planName, string intPath)
        {
            FDRunner fdRunner = new FDRunner(OptionsManager.Instance.FDPath, OptionsManager.Instance.PythonPrefix, OptionsManager.Instance.FDFileExecutionTimeout);
            return await fdRunner.RunAsync(
                _lastDomain,
                _lastProblem,
                _lastEngine,
                Path.Combine(outPath, $"{planName}.pddlplan"),
                Path.Combine(intPath, "intermediate.sas"));
        }

        private async Task WriteToOutputWindowAsync(FDResults resultData)
        {
            await OutputPanel.ActivateOutputWindowAsync();
            foreach (var item in resultData.Log)
            {
                if (item == null) continue;
                if (item.Type == LogItem.ItemType.Error)
                    await OutputPanel.WriteLineAsync($"[{item.Time}] (ERR) {item.Content}");
                if (item.Type == LogItem.ItemType.Log)
                    await OutputPanel.WriteLineAsync($"[{item.Time}]       {item.Content}");
            }
            switch (resultData.ResultReason)
            {
                case ProcessCompleteReson.ForceKilled:
                    await OutputPanel.WriteLineAsync($"ERROR! FD ran for longer than {OptionsManager.Instance.FDFileExecutionTimeout}! Killing process...");
                    break;
                case ProcessCompleteReson.StoppedOnError:
                    await OutputPanel.WriteLineAsync($"Errors encountered!");
                    break;
                case ProcessCompleteReson.RanToCompletion:
                    await OutputPanel.WriteLineAsync("FD ran to completion!");
                    break;
                case ProcessCompleteReson.ProcessNotRunning:
                    await OutputPanel.WriteLineAsync("Process is not running!");
                    break;
            }
        }

        private async Task SetupResultWindowsAsync(FDResults resultData, string domainFilePath, string problemFilePath, string planFile)
        {
            var vsShell = (IVsShell)ServiceProvider.GetService(typeof(IVsShell));
            if (vsShell.IsPackageLoaded(new Guid(PDDLTools.Constants.PackageGuidString), out var myPackage) == Microsoft.VisualStudio.VSConstants.S_OK)
            {
                var package = (PDDLToolsPackage)myPackage;
                if (OptionsManager.Instance.OpenResultReport)
                {
                    ToolWindowPane resultsWindow = await package.ShowToolWindowAsync(typeof(FDResultsWindow), 0, true, package.DisposalToken);
                    if ((null == resultsWindow) || (null == resultsWindow.Frame))
                    {
                        throw new NotSupportedException("Cannot create tool window");
                    }
                    if (resultsWindow.Content is FDResultsWindowControl control)
                        control.SetupResultData(resultData);
                }

                if (resultData.WasSolutionFound && OptionsManager.Instance.OpenSASSolutionVisualiser)
                {
                    PDDLParser.IPDDLParser parser = new PDDLParser.PDDLParser(true, false);
                    var pddlDoc = parser.TryParse(domainFilePath, problemFilePath);

                    ToolWindowPane sasWindow = await package.ShowToolWindowAsync(typeof(SASSolutionWindow), 0, true, package.DisposalToken);
                    if ((null == sasWindow) || (null == sasWindow.Frame))
                    {
                        throw new NotSupportedException("Cannot create tool window");
                    }
                    if (sasWindow.Content is SASSolutionWindowControl control)
                        control.SetupResultData(pddlDoc, planFile);
                }
            }
        }
    }
}
