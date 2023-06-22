using Microsoft.VisualStudio.Shell;
using PDDLTools.Windows.PDDLVisualiserWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal sealed class OpenPDDLVisualiserWindowCommand : BaseCommand
    {
        public override int CommandId { get; } = 270;
        public static OpenPDDLVisualiserWindowCommand Instance { get; internal set; }

        private OpenPDDLVisualiserWindowCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new OpenPDDLVisualiserWindowCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(PDDLVisualiserWindow), 0, true, this.package.DisposalToken);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }
        }
    }
}
