using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools
{
    [Guid(Constants.PDDLGuidEditorFactory)]
    [ComVisible(true)]
    internal class PDDLLanguageFactory : EditorFactory
    {
        public PDDLLanguageFactory(Package package) : base(package) { }
    }
}
