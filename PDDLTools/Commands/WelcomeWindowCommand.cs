using PDDLTools.Commands;
using PDDLTools.Helpers;
using PDDLTools.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using PDDLTools.Windows.WelcomeWindow;
using PDDLTools.Windows.PDDLVisualiserWindow;

namespace PDDLTools.Commands
{
    internal sealed class WelcomeWindowCommand : BaseCommand<WelcomeWindowCommand>
    {
        public override int CommandId { get; } = 262;

        private WelcomeWindowCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new WelcomeWindowCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            while (!await DTE2Helper.IsFullyVSOpenAsync())
            {
                if (_package.DisposalToken.IsCancellationRequested)
                    return;
                await Task.Delay(5000);
            }
            await Task.Delay(10000);
            var window = await OpenWindowOfTypeAsync(typeof(WelcomeWindow));
        }
    }
}
