using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Windows.PlanValidatorWindow
{
    [Guid("CCFA6DD5-74A4-4861-8B76-23EEB48B606B")]
    public class PlanValidatorWindow : ToolWindowPane
    {
        public PlanValidatorWindow() : base(null)
        {
            this.Caption = "PDDL Tools - Plan Validator";

            this.Content = new PlanValidatorWindowControl();
        }
    }
}
