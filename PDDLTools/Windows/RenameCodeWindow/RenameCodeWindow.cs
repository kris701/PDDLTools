using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Windows.RenameCodeWindow
{
    [Guid("5236A2AB-FC89-4FD2-80BF-3D630F71EA68")]
    public class RenameCodeWindow : ToolWindowPane
    {
        public RenameCodeWindow() : base(null)
        {
            this.Caption = "PDDL Tools - Rename";

            this.Content = new RenameCodeWindowControl();
        }
    }
}
