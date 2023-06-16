using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Options
{
    [Guid("5AF73C7B-2C8C-4696-9D23-F44CA69DB962")]
    public class DebugOptionsPageGrid : DialogPage
    {
        [Category("PDDL Tools")]
        [DisplayName("Is First Start")]
        [Description("")]
        public bool IsFirstStart => OptionsManager.Instance.IsFirstStart;
    }
}
