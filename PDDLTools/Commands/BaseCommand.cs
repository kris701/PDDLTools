using PDDLTools.Helpers;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using PDDLTools.Windows.PDDLVisualiserWindow;

namespace PDDLTools.Commands
{
    internal abstract class BaseCommand<T> : IDisposable
    {
        public static T Instance { get; internal set; }
        public abstract int CommandId { get; }
        public static readonly Guid CommandSet = new Guid(Constants.CommandSetGuid);
        public bool CanBeDisabled { get; } = true;

        internal readonly AsyncPackage _package;
        internal readonly OleMenuCommand _command;

        public BaseCommand(AsyncPackage package, OleMenuCommandService commandService, bool canBeDisabled = true)
        {
            CanBeDisabled = canBeDisabled;
            this._package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            _command = new OleMenuCommand(this.Execute, menuCommandID);
            if (CanBeDisabled)
                _command.BeforeQueryStatus += CheckQueryStatus;
            commandService.AddCommand(_command);
        }

        public static async Task<OleMenuCommandService> InitializeCommandServiceAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var newService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            return newService;
        }

        public virtual void Execute(object sender, EventArgs e)
        {
            this._package.JoinableTaskFactory.RunAsync(async delegate
            {
                await ExecuteAsync(sender, e);
            });
        }

        public virtual async Task ExecuteAsync(object sender, EventArgs e)
        {
            await Task.Delay(0);
        }

        public virtual async void CheckQueryStatus(object sender, EventArgs e)
        {
            var button = (MenuCommand)sender;
            button.Enabled = await DTE2Helper.IsValidFileOpenAsync();
        }

        public void Dispose()
        {
            this._command.BeforeQueryStatus -= this.CheckQueryStatus;
        }

        public void SetToggleState(bool toState)
        {
            _command.Checked = toState;
        }

        public async Task<ToolWindowPane> OpenWindowOfTypeAsync(Type type)
        {
            ToolWindowPane window = await this._package.ShowToolWindowAsync(type, 0, true, this._package.DisposalToken);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }
            return window;
        }
    }
}
