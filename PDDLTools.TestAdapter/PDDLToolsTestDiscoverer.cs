using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PDDLTools.TestAdapter
{
    [FileExtension(".pddltestproj")]
    [DirectoryBasedTestDiscoverer]
    [DefaultExecutorUri(PDDLToolsTestExecutor.ExecutorUri)]
    public sealed class PDDLToolsTestDiscoverer : PDDLTestAdapter, ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger messageLogger, ITestCaseDiscoverySink discoverySink)
        {
            TestLog.Initialize(messageLogger);
            Initialize(discoveryContext);
            TestLog.SendMessage(TestMessageLevel.Informational, "works!");
            foreach (var file in sources)
                TestLog.SendMessage(TestMessageLevel.Informational, $"   File: {file}");
        }
    }
}