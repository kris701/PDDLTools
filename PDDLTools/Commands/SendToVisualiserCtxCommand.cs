using Microsoft.VisualStudio.Shell;
using PDDLParser.Helpers;
using PDDLTools.Helpers;
using PDDLTools.Windows.PDDLVisualiserWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal class SendToVisualiserCtxCommand : BaseCommand
    {
        public override int CommandId { get; } = 273;
        public static SendToVisualiserCtxCommand Instance { get; internal set; }

        private SendToVisualiserCtxCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, true)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SendToVisualiserCtxCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async void CheckQueryStatus(object sender, EventArgs e)
        {
            if (sender is MenuCommand button)
            {
                var selected = await DTE2Helper.GetSourceFilePathFromSolutionExploreAsync();
                if (selected != null)
                    button.Visible = PDDLHelper.IsFileDomain(selected);
            }
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var selected = await DTE2Helper.GetSourceFilePathFromSolutionExploreAsync();
            if (selected != null && PDDLHelper.IsFileDomain(selected)) 
            {
                ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(PDDLVisualiserWindow), 0, true, this.package.DisposalToken);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
                if (window.Content is PDDLVisualiserWindowControl control)
                    control.SelectedDomainFile = selected;
            }
        }
    }
}
