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
    internal sealed class SASVisualiserCommand : BaseCommand
    {
        public override int CommandId { get; } = 268;
        public static SASVisualiserCommand Instance { get; internal set; }
        public static string SelectedDomainPath { get; internal set; } = "";

        private SASVisualiserCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new SASVisualiserCommand(package, await InitializeCommandServiceAsync(package));
            Instance.SetToggleState(OptionsAccessor.OpenSASSolutionVisualiser);
        }

        public override void Execute(object sender, EventArgs e)
        {
            OptionsAccessor.OpenSASSolutionVisualiser = !OptionsAccessor.OpenSASSolutionVisualiser;
            SetToggleState(OptionsAccessor.OpenSASSolutionVisualiser);
        }
    }
}
