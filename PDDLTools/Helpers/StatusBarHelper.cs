using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Helpers
{
    public class StatusBarHelper
    {
        public async Task ShowTextAsync(string text)
        {
            IVsStatusbar statusBar = (IVsStatusbar)(await ServiceProvider.GetGlobalServiceAsync(typeof(SVsStatusbar)));
            int frozen;
            statusBar.IsFrozen(out frozen);
            if (frozen != 0)
                statusBar.FreezeOutput(0);

            statusBar.SetText(text);

            statusBar.FreezeOutput(1);
        }

        public async Task ClearAsync()
        {
            IVsStatusbar statusBar = (IVsStatusbar)(await ServiceProvider.GetGlobalServiceAsync(typeof(SVsStatusbar)));
            statusBar.Clear();
        }
    }
}
