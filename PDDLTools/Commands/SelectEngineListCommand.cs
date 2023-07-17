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
using PDDLParser.Helpers;

namespace PDDLTools.Commands
{
    internal sealed class SelectEngineListCommand : BaseCommand<SelectEngineListCommand>
    {
        public override int CommandId { get; } = 264;
        private HashSet<string> _engines;

        private SelectEngineListCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectEngineListCommand(package, await InitializeCommandServiceAsync(package));
            Instance._engines = new HashSet<string>();
            Instance._engines.AddRange(GetEngines());
            Instance._engines.AddRange(await GetAliasesAsync());
        }

        public override void Execute(object sender, EventArgs e)
        {
            if (e is OleMenuCmdEventArgs eventArgs)
            {
                IntPtr pOutValue = eventArgs.OutValue;
                if (pOutValue != IntPtr.Zero)
                    Marshal.GetNativeVariantForObject(_engines.ToArray(), pOutValue);
            }
        }

        private static async Task<HashSet<string>> GetAliasesAsync()
        {
            var aliases = new HashSet<string>();
            FDRunner runner = new FDRunner(OptionsManager.Instance.FDPath, OptionsManager.Instance.PythonPrefix, OptionsManager.Instance.FDFileExecutionTimeout);
            var shortAliases = await runner.GetAliasesAsync();
            foreach (var shortAlias in shortAliases)
                aliases.Add($"--alias {shortAlias}");

            return aliases;
        }

        private static HashSet<string> GetEngines()
        {
            var optionsStr = OptionsManager.Instance.EngineOptions;
            return optionsStr.Split(';').ToHashSet();
        }
    }
}
