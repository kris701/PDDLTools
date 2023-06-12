using PDDLTools.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace PDDLTools.Windows.SASSolutionWindow
{
    [Guid("75DE6252-6333-479C-9BCD-459F6F552499")]
    public class SASSolutionWindow : ToolWindowPane
    {
        public SASSolutionWindow() : base(null)
        {
            this.Caption = "PDDL Tools - SAS Solution";

            this.Content = new SASSolutionWindowControl();
        }
    }
}
