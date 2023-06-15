using PDDLTools.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal abstract class BaseCommand : IDisposable
    {
        public abstract int CommandId { get; }
        public static readonly Guid CommandSet = new Guid(Constants.CommandSetGuid);

        internal readonly AsyncPackage package;
        internal readonly OleMenuCommand command;

        public bool CanBeDisabled { get; } = true;

        public BaseCommand(AsyncPackage package, OleMenuCommandService commandService, bool canBeDisabled = true)
        {
            CanBeDisabled = canBeDisabled;
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            command = new OleMenuCommand(this.Execute, menuCommandID);
            if (CanBeDisabled)
                command.BeforeQueryStatus += CheckQueryStatus;
            commandService.AddCommand(command);
        }

        public static async Task<OleMenuCommandService> InitializeCommandServiceAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var newService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            return newService;
        }

        public virtual void Execute(object sender, EventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(async delegate
            {
                await ExecuteAsync(sender, e);
            });
        }

        public virtual async Task ExecuteAsync(object sender, EventArgs e)
        {
            await Task.Delay(0);
        }

        private async void CheckQueryStatus(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;
            button.Enabled = await DTE2Helper.IsValidFileOpenAsync();
        }

        public void Dispose()
        {
            this.command.BeforeQueryStatus -= this.CheckQueryStatus;
        }

        public void SetToggleState(bool toState)
        {
            command.Checked = toState;
        }
    }
}
