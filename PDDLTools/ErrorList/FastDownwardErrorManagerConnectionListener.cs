using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using PDDLTools.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.ErrorList
{
    [Export(typeof(ITextViewConnectionListener))]
    [ContentType(Constants.PDDLLanguageName)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class FastDownwardErrorManagerConnectionListener : ITextViewConnectionListener
    {
        public void SubjectBuffersConnected(ITextView textView, ConnectionReason reason, IReadOnlyCollection<ITextBuffer> subjectBuffers)
        {
            if (!OptionsManager.Instance.EnableErrorCheckingOnSave)
                return;

            if (FastDownwardErrorManager.Instance != null)
                FastDownwardErrorManager.Instance.Initialize(textView);
        }

        public void SubjectBuffersDisconnected(ITextView textView, ConnectionReason reason, IReadOnlyCollection<ITextBuffer> subjectBuffers)
        {
            if (!OptionsManager.Instance.EnableErrorCheckingOnSave)
                return;

            if (FastDownwardErrorManager.Instance != null)
                FastDownwardErrorManager.Instance.Dispose();
        }
    }
}
