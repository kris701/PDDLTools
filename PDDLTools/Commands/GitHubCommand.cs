using PDDLTools.Commands;
using Microsoft.VisualStudio.Shell;
using System;
using System.Threading.Tasks;

namespace PDDLTools.Commands
{
    internal sealed class GitHubCommand : BaseCommand<GitHubCommand>
    {
        public override int CommandId { get; } = 258;

        private GitHubCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new GitHubCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            await Task.Run(() => System.Diagnostics.Process.Start("https://github.com/kris701/PDDLTools"));
        }
    }
}
