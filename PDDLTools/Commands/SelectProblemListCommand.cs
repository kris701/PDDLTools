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
using System.IO;
using PDDLParser.Helpers;
using PDDLTools.FileMonitors;
using System.Linq;

namespace PDDLTools.Commands
{
    internal sealed class SelectProblemListCommand : BaseCommand<SelectProblemListCommand>
    {
        public override int CommandId { get; } = 266;
        public static string NoneFoundComboboxName = "No open valid PDDL problems found";

        private SelectProblemListCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectProblemListCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                IntPtr pOutValue = eventArgs.OutValue;
                if (pOutValue != IntPtr.Zero)
                {
                    if (ProjectFileMonitorService.Instance != null)
                    {
                        var project = await DTE2Helper.GetActiveProjectPathAsync();
                        var files = await ProjectFileMonitorService.Instance.GetProblemCacheAsync(project);
                        if (files.Count == 0)
                            files.Add(NoneFoundComboboxName);
                        Marshal.GetNativeVariantForObject(files.ToArray(), pOutValue);
                    }
                }
            }
        }
    }
}
