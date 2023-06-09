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
    [ProvideOptionPage(typeof(OptionPageGrid),
    "PDDL Tools", "Options", 0, 0, true)]
    [ProvideToolWindow(typeof(WelcomeWindow), Transient = true, Style = VsDockStyle.MDI, Width = 1200, Height = 800)]
    public sealed class PDDLToolsPackage : AsyncPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            OptionPageGrid page = (OptionPageGrid)GetDialogPage(typeof(OptionPageGrid));
            OptionsAccessor.Instance = page;

            await WelcomeWindowCommand.InitializeAsync(this);
            await GitHubCommand.InitializeAsync(this);

            if (OptionsAccessor.IsFirstStart)
#pragma warning disable VSTHRD103 // Call async methods when in an async method
                WelcomeWindowCommand.Instance.Execute(null, null);
#pragma warning restore VSTHRD103 // Call async methods when in an async method

            await RunPDDLFileCommand.InitializeAsync(this);
            await SelectDomainCommand.InitializeAsync(this);
            await SelectDomainListCommand.InitializeAsync(this);
            new FastDownwardErrorManager(this);
        }
    }
}
