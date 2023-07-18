using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Diagnostics;
using System.ComponentModel.Composition;
using PDDLTools.TestAdapter.EventWatchers;
using Microsoft.VisualStudio.Shell;
using PDDLTools.TestAdapter.EventWatchers.EventArgs;
using Microsoft.VisualStudio.Shell.Interop;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;

namespace PDDLTools.TestAdapter
{
    [Export(typeof(ITestContainerDiscoverer))]
    public class PDDLTestContainerDiscoverer : ITestContainerDiscoverer
    {
        private System.IServiceProvider _serviceProvider;

        [ImportingConstructor]
        public PDDLTestContainerDiscoverer([Import(typeof(SVsServiceProvider))] System.IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Uri ExecutorUri
        {
            get { return new Uri(PDDLToolsTestExecutor.ExecutorUri); }
        }

        public IEnumerable<ITestContainer> TestContainers
        {
            get
            {
                return GetTestContainers();
            }
        }

        public event EventHandler TestContainersUpdated;

        private IEnumerable<ITestContainer> GetTestContainers()
        {
            var containers = new List<ITestContainer>();
            var pddlProjects = FindPDDLProjectFiles();
            foreach (var project in pddlProjects)
                containers.Add(new PDDLTestContainer(this, project, ExecutorUri));
            return containers;
        }

        private IEnumerable<string> FindPDDLProjectFiles()
        {
            var solution = (IVsSolution)_serviceProvider.GetService(typeof(SVsSolution));
            var loadedProjects = solution.EnumerateLoadedProjects(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();
            List<string> validProjects = new List<string>();
            foreach (var project in loadedProjects)
            {
                string name = "";
                project.GetMkDocument(VSConstants.VSITEMID_ROOT, out name);
                if (name != null)
                    if (name.ToLower().EndsWith(".pddltestproj"))
                        validProjects.Add(name);
            }

            return validProjects;
        }
    }
}
