using Microsoft.VisualStudio.Shell;
using PDDLTools.Windows.PlanValidatorWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal sealed class OpenPlanValidatorWindowCommand : BaseCommand
    {
        public override int CommandId { get; } = 269;
        public static OpenPlanValidatorWindowCommand Instance { get; internal set; }

        private OpenPlanValidatorWindowCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new OpenPlanValidatorWindowCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(PlanValidatorWindow), 0, true, this.package.DisposalToken);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }
            if (window.Content is PlanValidatorWindowControl control)
                await control.ReloadCheckerAsync();
        }
    }
}
