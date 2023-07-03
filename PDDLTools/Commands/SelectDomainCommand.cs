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
using System.Runtime.InteropServices;
using System.IO;
using PDDLParser.Helpers;
using System.Windows.Controls;
using PDDLTools.Projects;

namespace PDDLTools.Commands
{
    internal sealed class SelectDomainCommand : BaseCommand<SelectDomainCommand>
    {
        public override int CommandId { get; } = 257;

        private SelectDomainCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, true)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectDomainCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async void CheckQueryStatus(object sender, EventArgs e)
        {
            if (sender is MenuCommand button)
                button.Enabled = await DTE2Helper.IsActiveProjectPDDLProjectAsync();
        }

        private string _tempSelect = "";
        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var proj = await PDDLProjectManager.GetCurrentProjectAsync();
            if (proj != null)
            {
                OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
                if (eventArgs.InValue != null)
                {
                    if (eventArgs.InValue is string selected)
                    {
                        if (PDDLHelper.IsFileDomain(selected))
                        {
                            await proj.SetSelectedDomainAsync(selected);
                            _tempSelect = new FileInfo(selected).Name;
                        }
                    }
                }
                if (eventArgs.OutValue != null)
                {
                    IntPtr pOutValue = eventArgs.OutValue;
                    if (pOutValue != IntPtr.Zero)
                        Marshal.GetNativeVariantForObject(_tempSelect, pOutValue);
                }
            }
        }
    }
}
