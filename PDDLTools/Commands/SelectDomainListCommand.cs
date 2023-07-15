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
    internal sealed class SelectDomainListCommand : BaseCommand<SelectDomainListCommand>
    {
        public override int CommandId { get; } = 259;
        public static string NoneFoundComboboxName = "No open valid PDDL domains found";

        private SelectDomainListCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectDomainListCommand(package, await InitializeCommandServiceAsync(package));
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
                        var files = await ProjectFileMonitorService.Instance.GetDomainCacheAsync(project);
                        if (files.Count == 0)
                            files.Add(NoneFoundComboboxName);
                        else if (OptionsManager.Instance != null && files.Count > OptionsManager.Instance.DropDownListLimit)
                        {
                            files = files.Take(OptionsManager.Instance.DropDownListLimit).ToHashSet();
                            files.Add("Too many files to display here!");
                        }
                        Marshal.GetNativeVariantForObject(files.ToArray(), pOutValue);
                    }
                }
            }
        }

    }
}
