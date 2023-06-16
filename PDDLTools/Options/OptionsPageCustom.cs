﻿using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDDLTools.Options
{
    [Guid("1D32BA32-4249-4291-9539-1CB4C9FE9C88")]
    public class OptionsPageCustom : DialogPage
    {
        protected override IWin32Window Window
        {
            get
            {
                return new OptionsPageCustomControl();
            }
        }
    }
}
