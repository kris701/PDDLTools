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
using System.Windows.Controls;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using CMDRunners.FastDownward;

namespace PDDLTools.Commands
{
    internal sealed class SelectEngineListCommand : BaseCommand
    {
        public override int CommandId { get; } = 264;
        public static SelectEngineListCommand Instance { get; internal set; }

        private SelectEngineListCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectEngineListCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs eventArgs)
            {
                IntPtr pOutValue = eventArgs.OutValue;
                if (pOutValue != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(new string[] { "Loading..." }, pOutValue);

                    List<string> combined = new List<string>();
                    combined.AddRange(GetEngines());
                    combined.AddRange(await GetAliasesAsync());
                    Marshal.GetNativeVariantForObject(combined.ToArray(), pOutValue);
                }
            }
        }

        private List<string> _aliases = new List<string>();
        private async Task<List<string>> GetAliasesAsync()
        {
            if (_aliases == null)
                _aliases = new List<string>();
            if (_aliases.Count != 0)
                return _aliases;
            FDRunner runner = new FDRunner(OptionsManager.Instance.FDPath, OptionsManager.Instance.PythonPrefix, OptionsManager.Instance.FDFileExecutionTimeout);
            var shortAliases = await runner.GetAliasesAsync();
            foreach (var shortAlias in shortAliases)
                _aliases.Add($"--alias {shortAlias}");

            return _aliases;
        }

        private List<string> GetEngines()
        {
            var optionsStr = OptionsManager.Instance.EngineOptions;
            return optionsStr.Split(';').ToList();
        }
    }
}
