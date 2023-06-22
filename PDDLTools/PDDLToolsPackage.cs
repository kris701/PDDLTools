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

            await RunPDDLFileCommand.InitializeAsync(this);

            await SelectDomainCommand.InitializeAsync(this);
            await SelectDomainListCommand.InitializeAsync(this);

            await SelectProblemCommand.InitializeAsync(this);
            await SelectProblemListCommand.InitializeAsync(this);

            await SelectEngineCommand.InitializeAsync(this);
            await SelectEngineListCommand.InitializeAsync(this);

            await FDReportCommand.InitializeAsync(this);
            await SASVisualiserCommand.InitializeAsync(this);

            new FastDownwardErrorManager(this);
        }
    }
}
