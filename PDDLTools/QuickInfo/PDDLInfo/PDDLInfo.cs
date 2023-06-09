using Microsoft.VisualStudio.Text.Adornments;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.QuickInfo.PDDLInfo
{
    public static class PDDLInfo
    {
        public static bool IsLoading = false;
        public static Dictionary<string, ContainerElement> PreludeContent = new Dictionary<string, ContainerElement>();
    }
}
