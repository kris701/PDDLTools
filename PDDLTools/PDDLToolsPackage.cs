using EnvDTE;
using PDDLTools.Commands;
using PDDLTools.ErrorList;
using PDDLTools.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.IO.Packaging;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;
using PDDLTools.Windows.FDResultsWindow;
using PDDLTools.Windows.SASSolutionWindow;
using PDDLTools.Windows.WelcomeWindow;
using PDDLTools.Windows.PlanValidatorWindow;
using PDDLTools.Windows.PDDLVisualiserWindow;
using PDDLTools.Projects;
using PDDLTools.Helpers;
using System.Collections.Generic;
using System.Reflection.Metadata;
using PDDLTools.Windows.RenameCodeWindow;
using PDDLTools.FileMonitors;
using Microsoft.VisualStudio.ProjectSystem;
using System.IO;

namespace PDDLTools
{
    [ProvideService(typeof(PDDLLanguageFactory), ServiceName = nameof(PDDLLanguageFactory))]
    [ProvideLanguageService(typeof(PDDLLanguageFactory), Constants.PDDLLanguageName, Constants.PDDLLanguageID,
        ShowHotURLs = false, DefaultToNonHotURLs = true, EnableLineNumbers = true,
        EnableAsyncCompletion = true, EnableCommenting = true, ShowCompletion = true,
        AutoOutlining = true, CodeSense = true, RequestStockColors = true, EnableFormatSelection = true,
        QuickInfo = true, ShowDropDownOptions = true, ShowMatchingBrace = true
        )]
    [ProvideLanguageExtension(typeof(PDDLLanguageFactory), Constants.PDDLExt)]

    [ProvideUIContextRule(Constants.PackageGuidString,
        name: "Supported Files",
        expression: "pddl",
        termNames: new[] { "pddl", "pddlplan" },
        termValues: new[] { "HierSingleSelectionName:.pddl$", "HierSingleSelectionName:.pddlplan$" })]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(Constants.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionsPageCustom), "PDDL Tools", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(DebugOptionsPageGrid), "PDDL Tools", "Debug", 0, 0, true)]
    [ProvideToolWindow(typeof(FDResultsWindow), Transient = true, Style = VsDockStyle.Tabbed, Window = EnvDTE.Constants.vsWindowKindOutput, Width = 1200, Height = 800)]
    [ProvideToolWindow(typeof(PlanValidatorWindow), Transient = true, Style = VsDockStyle.Tabbed, Window = EnvDTE.Constants.vsWindowKindOutput, Width = 1200, Height = 800)]
    [ProvideToolWindow(typeof(SASSolutionWindow), Transient = true, Style = VsDockStyle.MDI, Width = 1200, Height = 800)]
    [ProvideToolWindow(typeof(PDDLVisualiserWindow), Transient = true, Style = VsDockStyle.MDI, Width = 1200, Height = 800)]
    [ProvideToolWindow(typeof(WelcomeWindow), Transient = true, Style = VsDockStyle.MDI, Width = 1200, Height = 800)]
    [ProvideToolWindow(typeof(RenameCodeWindow), Transient = true, Style = VsDockStyle.Float, Width = 500, Height = 120)]
    public sealed class PDDLToolsPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            OptionsManager.Instance = new OptionsManager();
            await OptionsManager.Instance.LoadSettingsAsync();

            await WelcomeWindowCommand.InitializeAsync(this);
            await GitHubCommand.InitializeAsync(this);
            await OpenPlanValidatorWindowCommand.InitializeAsync(this);
            await OpenPDDLVisualiserWindowCommand.InitializeAsync(this);

            if (OptionsManager.Instance.IsFirstStart)
#pragma warning disable VSTHRD103 // Call async methods when in an async method
                WelcomeWindowCommand.Instance.Execute(null, null);
#pragma warning restore VSTHRD103 // Call async methods when in an async method

            await SelectDomainCommand.InitializeAsync(this);
            await SelectDomainCtxCommand.InitializeAsync(this);
            await SelectDomainListCommand.InitializeAsync(this);
            await SelectProblemCommand.InitializeAsync(this);
            await SelectProblemCtxCommand.InitializeAsync(this);
            await SelectProblemListCommand.InitializeAsync(this);

            await SelectEngineCommand.InitializeAsync(this);
            await SelectEngineListCommand.InitializeAsync(this);

            await SendToVisualiserCtxCommand.InitializeAsync(this);

            await SendDomainToValidatorCtxCommand.InitializeAsync(this);
            await SendProblemToValidatorCtxCommand.InitializeAsync(this);
            await SendPlanToValidatorCtxCommand.InitializeAsync(this);

            await FDReportCommand.InitializeAsync(this);
            await SASVisualiserCommand.InitializeAsync(this);

            await RenameCodeCommand.InitializeAsync(this);

            new FastDownwardErrorManager(this);

            new ProjectFileMonitorService();

            var dte2 = DTE2Helper.GetDTE2();
            var docEvent = dte2.Events.CommandEvents;
            events.Add(docEvent);
            docEvent.AfterExecute += LoadPropertiesIntoVS;
        }

        // This is a rather expensive method. But i couldnt find any other way of attaching an event to the "Set as Startup Project" command
        List<object> events = new List<object>();
        private async void LoadPropertiesIntoVS(string Guid, int ID, object CustomIn, object CustomOut)
        {
            if (ID == 246 && Guid == "{5EFC7975-14BC-11CF-9B2B-00AA00573819}")
            {
                var proj = await PDDLProjectManager.GetCurrentProjectAsync();
                if (proj != null)
                {
                    if (SelectDomainCommand.Instance != null)
                        await SelectDomainCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(await proj.GetSelectedDomainAsync(), IntPtr.Zero));
                    if (SelectProblemCommand.Instance != null)
                        await SelectProblemCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(await proj.GetSelectedProblemAsync(), IntPtr.Zero));
                    if (SelectEngineCommand.Instance != null)
                        await SelectEngineCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(await proj.GetSelectedEngineAsync(), IntPtr.Zero));
                    if (ProjectFileMonitorService.Instance != null)
                        await ProjectFileMonitorService.Instance.InitialiseAsync(new FileInfo(proj.ConfiguredProject.UnconfiguredProject.FullPath).Directory.FullName);
                }
            }
        }
    }
}
