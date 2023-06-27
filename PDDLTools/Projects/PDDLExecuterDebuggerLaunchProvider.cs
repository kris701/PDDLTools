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
    using PDDLParser;
    using PDDLParser.Helpers;
    using PDDLTools.Commands;
    using PDDLTools.Helpers;
    using PDDLTools.Options;
    using PDDLTools.Windows.FDResultsWindow;
    using PDDLTools.Windows.SASSolutionWindow;
    using static Microsoft.VisualStudio.VSConstants;

    [ExportDebugger("PDDLExecuter")]
    [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
    public class PDDLExecuterDebuggerLaunchProvider : DebugLaunchProviderBase
    {
        [Import]
        private ProjectProperties ProjectProperties { get; set; }
        private bool _isLoaded = false;

        private static OutputPanelController OutputPanel = new OutputPanelController("Fast Downward Output");
        private string _lastDomain = "";
        private string _lastProblem = "";
        private bool _lastCheckResult = false;

        [ImportingConstructor]
        public PDDLExecuterDebuggerLaunchProvider(ConfiguredProject configuredProject)
            : base(configuredProject)
        {
        }

        [ExportPropertyXamlRuleDefinition("PDDL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9be6e469bc4921f1", "XamlRuleToCode:PDDLExecuterDebugger.xaml", "Project")]
        [AppliesTo(PDDLUnconfiguredProject.UniqueCapability)]
        private object DebuggerXaml { get { throw new NotImplementedException(); } }

        private async Task LoadFromSavedProjectPropertiesAsync()
        {
            var generalProps = await ProjectProperties.GetConfigurationGeneralPropertiesAsync();
            var lastDomain = await generalProps.SelectedDomain.GetValueAsync();
            var lastProblem = await generalProps.SelectedProblem.GetValueAsync();
            var lastEngine = await generalProps.SelectedEngine.GetValueAsync();

            if (lastDomain != null && lastDomain is string domainFile)
                if (PDDLHelper.IsFileDomain(domainFile))
                    SelectDomainCommand.SelectedDomainPath = domainFile;
            if (lastProblem != null && lastProblem is string problemFile)
                if (PDDLHelper.IsFileProblem(problemFile))
                    SelectProblemCommand.SelectedProblemPath = problemFile;
            if (lastEngine != null && lastEngine is string engineStr)
                SelectEngineCommand.SelectedSearch = engineStr;
            _isLoaded = true;
        }

        public override async Task<bool> CanLaunchAsync(DebugLaunchOptions launchOptions)
        {
            if (!_isLoaded)
                await LoadFromSavedProjectPropertiesAsync();
            if (_lastDomain != SelectDomainCommand.SelectedDomainPath || _lastProblem != SelectProblemCommand.SelectedProblemPath)
            {
                _lastDomain = SelectDomainCommand.SelectedDomainPath;
                _lastProblem = SelectProblemCommand.SelectedProblemPath;
                _lastCheckResult = PDDLHelper.IsFileDomain(_lastDomain) && PDDLHelper.IsFileProblem(_lastProblem);
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
            await OutputPanel.InitializeAsync();
            await OutputPanel.ClearOutputAsync();
            await OutputPanel.WriteLineAsync("Checking if files are valid...");

            bool canLaunch = false;
            try
            {
                IPDDLParser parser = new PDDLParser();
                parser.Parse(_lastDomain, _lastProblem);
                canLaunch = true;
            }
            catch 
            {
                await OutputPanel.WriteLineAsync("There are parse errors in the execution files!");
            }

            if (canLaunch)
            {
                await OutputPanel.WriteLineAsync("Executing PDDL File");
                FDRunner fdRunner = new FDRunner(OptionsManager.Instance.FDPath, OptionsManager.Instance.PythonPrefix, OptionsManager.Instance.FDFileExecutionTimeout);
                var resultData = await fdRunner.RunAsync(_lastDomain, _lastProblem, SelectEngineCommand.SelectedSearch);

                await WriteToOutputWindowAsync(resultData);
                if (resultData.ResultReason == ProcessCompleteReson.RanToCompletion)
                    await SetupResultWindowsAsync(resultData, _lastDomain, _lastProblem);
            }
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

        private async Task SetupResultWindowsAsync(FDResults resultData, string domainFilePath, string problemFilePath)
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
                    IPDDLParser parser = new PDDLParser(false, false);
                    var pddlDoc = parser.Parse(domainFilePath, problemFilePath);

                    ToolWindowPane sasWindow = await package.ShowToolWindowAsync(typeof(SASSolutionWindow), 0, true, package.DisposalToken);
                    if ((null == sasWindow) || (null == sasWindow.Frame))
                    {
                        throw new NotSupportedException("Cannot create tool window");
                    }
                    if (sasWindow.Content is SASSolutionWindowControl control)
                        control.SetupResultData(pddlDoc);
                }
            }
        }
    }
}
