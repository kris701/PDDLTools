using PDDLTools.Commands;
using PDDLTools.Helpers;
using PDDLTools.Options;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.IO.Packaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Task = System.Threading.Tasks.Task;

namespace PDDLTools.Commands
{
    internal sealed class RunPDDLFileCommand : BaseCommand
    {
        private static OutputPanelController OutputPanel = new OutputPanelController("Fast Downward Output");
        public override int CommandId { get; } = 256;
        public static RunPDDLFileCommand Instance { get; internal set; }
        private PowershellProcess _process;
        private bool _isRunning = false;

        private string _sourceFilePath = "";

        private RunPDDLFileCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new RunPDDLFileCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            if (_isRunning)
            {
                return;
            }

            if (!await DTE2Helper.IsValidFileOpenAsync())
            {
                MessageBox.Show("File must be a '.pddl' file!");
                return;
            }
            await DTE2Helper.SaveActiveDocumentAsync();

            _sourceFilePath = await DTE2Helper.GetSourceFilePathAsync();

            _isRunning = true;

            await OutputPanel.InitializeAsync();
            await OutputPanel.ClearOutputAsync();
            await OutputPanel.WriteLineAsync("Executing PDDL File");
            await RunAsync();
        }

        private async Task RunAsync()
        {
            _process = new PowershellProcess();
            _process.ErrorDataRecieved += RecieveErrorData;
            _process.OutputDataRecieved += RecieveOutputData;
            _process.StopOnError = true;
             await _process.StartProcessAsync($"& 'python {DirHelper.CombinePathAndFile(OptionsAccessor.FDPPath, "fastdownward.py")}' '{_sourceFilePath}'");

            var timeoutSpan = TimeSpan.FromSeconds(OptionsAccessor.FDFileExecutionTimeout);
            var res = await _process.WaitForExitAsync(timeoutSpan);
            await OutputPanel.ActivateOutputWindowAsync();
            switch (res)
            {
                case ProcessCompleteReson.ForceKilled:
                    await OutputPanel.WriteLineAsync($"ERROR! Function ran for longer than {timeoutSpan}! Killing process...");
                    break;
                case ProcessCompleteReson.StoppedOnError:
                    await OutputPanel.WriteLineAsync($"Errors encountered!");
                    break;
                case ProcessCompleteReson.RanToCompletion:
                    await OutputPanel.WriteLineAsync("Function ran to completion!");
                    break;
                case ProcessCompleteReson.ProcessNotRunning:
                    await OutputPanel.WriteLineAsync("Process is not running!");
                    break;
            }
            _isRunning = false;
        }

        private async void RecieveErrorData(object sender, DataReceivedEventArgs e)
        {
            await OutputPanel.WriteLineAsync($"ERROR! {e.Data}");
        }

        private async void RecieveOutputData(object sender, DataReceivedEventArgs e)
        {
            await OutputPanel.WriteLineAsync($"{e.Data}");
        }
    }
}
