using PDDLTools.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace PDDLTools.Windows.FDResultsWindow
{
    [Guid("239B33FA-D0B3-44D5-8850-6469F0B76F57")]
    public class FDResultsWindow : ToolWindowPane
    {
        public FDResultsWindow() : base(null)
        {
            this.Caption = "PDDL Tools - Fast Downward Results";

            this.Content = new FDResultsWindowControl();
        }
    }
}
