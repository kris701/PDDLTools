using PDDLTools.Commands;
using PDDLTools.Helpers;
using PDDLTools.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO.Packaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.IO;
using PDDLParser.Helpers;
using PDDLTools.FileMonitors;

namespace PDDLTools.Commands
{
    internal sealed class SelectProblemCtxCommand : BaseCommand<SelectProblemCtxCommand>
    {
        public override int CommandId { get; } = 271;

        private SelectProblemCtxCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, true)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectProblemCtxCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async void CheckQueryStatus(object sender, EventArgs e)
        {
            if (sender is MenuCommand button)
            {
                var selected = await DTE2Helper.GetSourceFilePathFromSolutionExploreAsync();
                var project = await DTE2Helper.GetActiveProjectPathAsync();
                var files = await ProjectFileMonitorService.Instance.GetProblemCacheAsync(project);
                if (selected != null)
                    button.Visible = files.Contains(selected);
            }
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var selected = await DTE2Helper.GetSourceFilePathFromSolutionExploreAsync();
            if (selected != null)
                if (PDDLHelper.IsFileProblem(selected))
                    await SelectProblemCommand.Instance.ExecuteAsync(null, new OleMenuCmdEventArgs(selected, IntPtr.Zero));
        }
    }
}
