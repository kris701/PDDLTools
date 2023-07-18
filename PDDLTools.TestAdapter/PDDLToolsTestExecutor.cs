using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDDLTools.TestAdapter
{
    [ExtensionUri(ExecutorUri)]
    public sealed class PDDLToolsTestExecutor : PDDLTestAdapter, ITestExecutor
    {
        public const string ExecutorUri = "executor://PDDLToolsTestExecutor/v1";

        public void Cancel()
        {
            MessageBox.Show("test");
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            MessageBox.Show("test");
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            MessageBox.Show("test");
        }
    }
}
