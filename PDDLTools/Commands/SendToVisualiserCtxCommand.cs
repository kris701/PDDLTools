using Microsoft.VisualStudio.Shell;
using PDDLParser.Helpers;
using PDDLTools.Helpers;
using PDDLTools.Windows.PDDLVisualiserWindow;
using PDDLTools.Windows.PlanValidatorWindow;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal class SendToVisualiserCtxCommand : BaseCommand<SendToVisualiserCtxCommand>
    {
        public override int CommandId { get; } = 273;

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
                var window = await OpenWindowOfTypeAsync(typeof(PDDLVisualiserWindow));
                if (window.Content is PDDLVisualiserWindowControl control)
                {
                    while (!control.IsUILoaded)
                        await Task.Delay(100);
                    control.SelectedDomainFile = selected;
                }
            }
        }
    }
}
