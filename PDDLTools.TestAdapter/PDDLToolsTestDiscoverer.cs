using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace PDDLTools.TestAdapter
{
    [FileExtension(".pddl")]
    [DefaultExecutorUri(PDDLToolsTestExecutor.ExecutorUri)]
    public sealed class PDDLToolsTestDiscoverer : PDDLTestAdapter, ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger messageLogger, ITestCaseDiscoverySink discoverySink)
        {
            TestLog.Initialize(messageLogger);
            Initialize(discoveryContext);
            if (RegistryFailure)
            {
                TestLog.SendErrorMessage(ErrorMsg);
            }
            Info("discovering tests", "started");

            foreach(var source in sources)
            {
                TestLog.SendMessage(TestMessageLevel.Informational, source);
            }
        }
    }
}