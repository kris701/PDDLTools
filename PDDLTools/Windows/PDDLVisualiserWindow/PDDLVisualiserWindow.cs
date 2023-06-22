using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Windows.PDDLVisualiserWindow
{
    public class PDDLVisualiserWindow : ToolWindowPane
    {
        public PDDLVisualiserWindow() : base(null)
        {
            this.Caption = "PDDL Tools - PDDL Visualiser";

            this.Content = new PDDLVisualiserWindowControl();
        }
    }
}
