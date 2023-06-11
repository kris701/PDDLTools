using PDDLTools.Helpers;
using PDDLTools.Options;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;

namespace PDDLTools.ErrorList
{
    internal class FastDownwardErrorManager
    {
        public static FastDownwardErrorManager Instance;
        public string FileName { get; set; }
        public ITextView TextField { get; set; }

        private ErrorListProvider _errorProvider;

        List<object> events = new List<object>();

        public FastDownwardErrorManager(PDDLToolsPackage package)
        {
            Instance = this;
            _errorProvider = new ErrorListProvider(package);

            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = DTE2Helper.GetDTE2();
            var docEvent = dte2.Events.DocumentEvents;
            events.Add(docEvent);
            docEvent.DocumentSaved += CheckDocument;
        }

        public void Initialize(ITextView textField)
        {
            TextField = textField;
            CheckDocument(null);
        }

        public void Dispose()
        {
            _errorProvider.Tasks.Clear();
        }

        public async void CheckDocument(EnvDTE.Document document)
        {
            _errorProvider.Tasks.Clear();

            ErrorTask newError = new ErrorTask();
            newError.ErrorCategory = TaskErrorCategory.Error;
            newError.Text = "test error";
            newError.Line = 0;
            newError.Navigate += JumpToError;
            newError.Document = "";
            newError.Priority = TaskPriority.High;

            _errorProvider.Tasks.Add(newError);

            if (_errorProvider.Tasks.Count > 0)
                _errorProvider.Show();
        }

        private async void JumpToError(object sender, EventArgs e)
        {
            if (sender is ErrorTask item) {
                foreach(var line in TextField.TextViewLines)
                {
                    var lineText = line.Extent.GetText().Trim();
                    if (lineText == item.Document)
                    {
                        var newSpan = new Microsoft.VisualStudio.Text.SnapshotSpan(line.Extent.Snapshot, line.Extent.Span);
                        TextField.Selection.Select(newSpan, false);
                        await DTE2Helper.FocusActiveDocumentAsync();
                        break;
                    }
                }
            }
        }
    }
}
