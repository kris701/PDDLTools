using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.TestAdapter
{
    [ExtensionUri(ExecutorUri)]
    public sealed class PDDLToolsTestExecutor : PDDLTestAdapter, ITestExecutor
    {
        public const string ExecutorUri = "executor://PDDLToolsTestExecutor";

        public void Cancel()
        {
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
        }
    }
}
