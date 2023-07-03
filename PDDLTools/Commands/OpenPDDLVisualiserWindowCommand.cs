using Microsoft.VisualStudio.Shell;
using PDDLTools.Windows.PDDLVisualiserWindow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal sealed class OpenPDDLVisualiserWindowCommand : BaseCommand<OpenPDDLVisualiserWindowCommand>
    {
        public override int CommandId { get; } = 270;

        private OpenPDDLVisualiserWindowCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new OpenPDDLVisualiserWindowCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            await OpenWindowOfTypeAsync(typeof(PDDLVisualiserWindow));
        }
    }
}
