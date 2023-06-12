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
    internal sealed class SelectSearchCommand : BaseCommand
    {
        public override int CommandId { get; } = 263;
        public static SelectSearchCommand Instance { get; internal set; }
        public static string SelectedSearch { get; internal set; } = "";

        private SelectSearchCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SelectSearchCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            OleMenuCmdEventArgs eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs.InValue != null)
            {
                SelectedSearch = eventArgs.InValue as string;
            }
            if (eventArgs.OutValue != null && SelectedSearch != "")
            {
                IntPtr pOutValue = eventArgs.OutValue;
                if (pOutValue != IntPtr.Zero)
                {
                    Marshal.GetNativeVariantForObject(SelectedSearch, pOutValue);
                }
            }
        }
    }
}
