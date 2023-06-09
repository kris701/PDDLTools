using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Options
{
    public class OptionPageGrid : DialogPage
    {
        [Category("PDDL Tools")]
        [DisplayName("Optional Fast Downward Path")]
        [Description("Optional path to your Fast Downward installation folder. (Leave empty if environment variables is set). Note, restart of Visual Studio is required when changing this setting.")]
        [DefaultValue("")]
        public string FDPath { get; set; }

        [Category("PDDL Tools")]
        [DisplayName("Fast Downward File Execution Timeout")]
        [Description("How much time should pass before the Fast Downward instance is killed in seconds.")]
        [DefaultValue(10)]
        public int FDFileExecutionTimeout { get; set; } = 10;

        [Category("PDDL Tools")]
        [DisplayName("Is this the first time the extension starts?")]
        [Description("Indication that the extention have been installed.")]
        [DefaultValue(true)]
#if DEBUG
        public bool IsFirstStart { get; set; } = true;
#else
        public bool IsFirstStart { get; internal set; } = true;
#endif
    }
}
