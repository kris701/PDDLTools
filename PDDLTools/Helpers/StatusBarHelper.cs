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

        public async Task ShowProgressAsync(string text, uint value, uint max)
        {
            IVsStatusbar statusBar = (IVsStatusbar)(await ServiceProvider.GetGlobalServiceAsync(typeof(SVsStatusbar)));

            int frozen;
            statusBar.IsFrozen(out frozen);
            if (frozen != 0)
                statusBar.FreezeOutput(0);

            uint cookie = 0;
            statusBar.Progress(ref cookie, 1, "", value, max);
            statusBar.SetText(text);
        }

        public async Task ClearProgressAsync()
        {
            IVsStatusbar statusBar = (IVsStatusbar)(await ServiceProvider.GetGlobalServiceAsync(typeof(SVsStatusbar)));
            uint cookie = 0; 
            statusBar.Progress(ref cookie, 0, "", 0, 0);
            statusBar.FreezeOutput(0);
            statusBar.Clear();
        }

        public async Task ClearAsync()
        {
            IVsStatusbar statusBar = (IVsStatusbar)(await ServiceProvider.GetGlobalServiceAsync(typeof(SVsStatusbar)));
            statusBar.FreezeOutput(0); 
            statusBar.Clear();
        }
    }
}
