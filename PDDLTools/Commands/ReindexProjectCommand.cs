using Microsoft.VisualStudio.Shell;
using PDDLTools.FileMonitors;
using PDDLTools.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal sealed class ReindexProjectCommand : BaseCommand<ReindexProjectCommand>
    {
        public override int CommandId { get; } = 278;

        private ReindexProjectCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, true)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new ReindexProjectCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async void CheckQueryStatus(object sender, EventArgs e)
        {
            if (sender is MenuCommand button)
            {
                var projectNode = await DTE2Helper.GetSourceFilePathFromSolutionExploreAsync();
                if (projectNode != null)
                    button.Visible = projectNode.EndsWith(Constants.PDDLProjectExt);
            }
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            if (ProjectFileMonitorService.Instance != null)
            {
                var projectNode = await DTE2Helper.GetSourceFilePathFromSolutionExploreAsync();
                if (projectNode != null && projectNode.EndsWith(Constants.PDDLProjectExt))
                    await ProjectFileMonitorService.Instance.InitialiseAsync(projectNode);
            }
        }
    }
}
