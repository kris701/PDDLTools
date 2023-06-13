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
using HaskellTools.Helpers;
using System.Collections.Generic;
using PDDLTools.Windows.FDResultsWindow;
using PDDLTools.Windows.SASSolutionWindow;
using PDDLParser;
using FastDownwardRunner;
using FastDownwardRunner.Helpers;
using FastDownwardRunner.Models;

namespace PDDLTools.Commands
{
    internal sealed class RunPDDLFileCommand : BaseCommand
    {
        private static OutputPanelController OutputPanel = new OutputPanelController("Fast Downward Output");
        public override int CommandId { get; } = 256;
        public static RunPDDLFileCommand Instance { get; internal set; }
        private bool _isRunning = false;

        private RunPDDLFileCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new RunPDDLFileCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            if (_isRunning)
                return;

            string domainFilePath = "";
            string problemFilePath = "";

            await DTE2Helper.SaveActiveDocumentAsync();

            if (SelectDomainCommand.SelectedDomainPath == SelectDomainListCommand.ActiveDocumentComboboxName)
            {
                var openDocument = await DTE2Helper.GetSourceFilePathAsync();
                if (!PDDLHelper.IsFileDomain(openDocument))
                {
                    MessageBox.Show("Active document must be a valid PDDL domain file!");
                    return;
                }
                domainFilePath = openDocument;
            }
            else
            {
                if (!PDDLHelper.IsFileDomain(SelectDomainCommand.SelectedDomainPath))
                {
                    MessageBox.Show("Selected document must be a valid PDDL domain file!");
                    return;
                }
                domainFilePath = SelectDomainCommand.SelectedDomainPath;
            }


            if (SelectProblemCommand.SelectedProblemPath == SelectProblemListCommand.ActiveDocumentComboboxName)
            {
                var openDocument = await DTE2Helper.GetSourceFilePathAsync();
                if (!PDDLHelper.IsFileProblem(openDocument))
                {
                    MessageBox.Show("Active document must be a valid PDDL problem file!");
                    return;
                }
                problemFilePath = openDocument;
            }
            else
            {
                if (!PDDLHelper.IsFileProblem(SelectProblemCommand.SelectedProblemPath))
                {
                    MessageBox.Show("Selected document must be a valid PDDL problem file!");
                    return;
                }
                problemFilePath = SelectProblemCommand.SelectedProblemPath;
            }

            if (SelectSearchCommand.SelectedSearch == "")
            {
                MessageBox.Show("Please select a search option.");
                return;
            }

            _isRunning = true;

            await OutputPanel.InitializeAsync();
            await OutputPanel.ClearOutputAsync();
            await OutputPanel.WriteLineAsync("Executing PDDL File");

            IRunner fdRunner = new FDRunner(OptionsAccessor.FDPPath, OptionsAccessor.PythonPrefix, OptionsAccessor.FDFileExecutionTimeout);
            var resultData = await fdRunner.RunAsync(domainFilePath, problemFilePath, SelectSearchCommand.SelectedSearch);

            await WriteToOutputWindowAsync(resultData);
            await SetupResultWindowsAsync(resultData, domainFilePath, problemFilePath);

            _isRunning = false;
        }

        private async Task WriteToOutputWindowAsync(FDResults resultData)
        {
            await OutputPanel.ActivateOutputWindowAsync();
            foreach(var item in resultData.Log)
            {
                if (item.Type == LogItem.ItemType.Error)
                    await OutputPanel.WriteLineAsync($"[{item.Time}] (ERR) {item.Content}");
                if (item.Type == LogItem.ItemType.Log)
                    await OutputPanel.WriteLineAsync($"[{item.Time}]       {item.Content}");
            }
            switch (resultData.ResultReason)
            {
                case ProcessCompleteReson.ForceKilled:
                    await OutputPanel.WriteLineAsync($"ERROR! FD ran for longer than {OptionsAccessor.FDFileExecutionTimeout}! Killing process...");
                    break;
                case ProcessCompleteReson.StoppedOnError:
                    await OutputPanel.WriteLineAsync($"Errors encountered!");
                    break;
                case ProcessCompleteReson.RanToCompletion:
                    await OutputPanel.WriteLineAsync("FD ran to completion!");
                    break;
                case ProcessCompleteReson.ProcessNotRunning:
                    await OutputPanel.WriteLineAsync("Process is not running!");
                    break;
            }
        }

        private async Task SetupResultWindowsAsync(FDResults resultData, string domainFilePath, string problemFilePath)
        {
            if (OptionsAccessor.OpenResultReport)
            {
                ToolWindowPane resultsWindow = await this.package.ShowToolWindowAsync(typeof(FDResultsWindow), 0, true, this.package.DisposalToken);
                if ((null == resultsWindow) || (null == resultsWindow.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
                await ((resultsWindow as FDResultsWindow).Content as FDResultsWindowControl).SetupResultDataAsync(resultData);
            }

            if (resultData.WasSolutionFound && OptionsAccessor.OpenSASSolutionVisualiser)
            {
                IPDDLParser parser = new PDDLParser.PDDLParser();
                var pddlDoc = parser.ParseDomainAndProblemFiles(domainFilePath, problemFilePath);

                ToolWindowPane sasWindow = await this.package.ShowToolWindowAsync(typeof(SASSolutionWindow), 0, true, this.package.DisposalToken);
                if ((null == sasWindow) || (null == sasWindow.Frame))
                {
                    throw new NotSupportedException("Cannot create tool window");
                }
                ((sasWindow as SASSolutionWindow).Content as SASSolutionWindowControl).SetupResultData(resultData, pddlDoc);
            }
        }
    }
}
