using PDDLTools.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace PDDLTools.Windows.WelcomeWindow
{
    [Guid("ad431514-27ad-42ad-a5c8-e7187a8ddebb")]
    public class WelcomeWindow : ToolWindowPane
    {
        public WelcomeWindow() : base(null)
        {
            this.Caption = "PDDL Tools - Welcome";

            this.Content = new WelcomeWindowControl();
        }
    }
}
