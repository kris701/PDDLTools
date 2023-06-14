﻿using Microsoft.VisualStudio.Shell;
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
        [DisplayName("Fast Downward Path")]
        [Description("Path to your Fast Downward installation folder. Note, restart of Visual Studio is required when changing this setting.")]
        [DefaultValue("")]
        public string FDPath { get; set; }

        [Category("PDDL Tools")]
        [DisplayName("Python Prefix")]
        [Description("What the 'python' command is called in your environment. Usually its 'python3' or 'python'")]
        [DefaultValue("")]
        public string PythonPrefix { get; set; } = "python";

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

        [Category("PDDL Tools")]
        [DisplayName("Fast Downward Search Options")]
        [Description("What options that is available to run with Fast Downward. Note, this is the '--search' parameter of FD. Semi-colon seperated!")]
        [DefaultValue("astar(lmcut());lazy_greedy([ff(), cea()], [ff(), cea()])")]
        public string SearchOptions { get; set; } = "astar(lmcut());lazy_greedy([ff(), cea()], [ff(), cea()])";

        [Category("PDDL Tools")]
        [DisplayName("General Result Report")]
        [Description("If the result of a FD run should be generated into a report.")]
        [DefaultValue(true)]
        public bool OpenResultReport { get; set; } = true;

        [Category("PDDL Tools")]
        [DisplayName("SAS Solution Visualiser")]
        [Description("If the resulting plan should be displayed in the visualiser.")]
        [DefaultValue(true)]
        public bool OpenSASSolutionVisualiser { get; set; } = true;
    }
}
