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

namespace PDDLTools.Commands
{
    internal sealed class SelectDomainCommand : BaseCommand
    {
        public override int CommandId { get; } = 257;
        public static SelectDomainCommand Instance { get; internal set; }
        public static string SelectedDomainPath { get; internal set; } = "";

        private SelectDomainCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectDomainCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs.InValue != null)
            {
                var selectedStr = eventArgs.InValue as string;
                if (selectedStr != SelectDomainListCommand.NoneFoundComboboxName)
                    SelectedDomainPath = selectedStr;
            }
            if (eventArgs.OutValue != null && SelectedDomainPath != "")
            {
                IntPtr pOutValue = eventArgs.OutValue;
                if (pOutValue != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(SelectedDomainPath, pOutValue);
                }
            }
        }
    }
}
