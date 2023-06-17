using PDDLTools.Options;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLTools.QuickInfo;

namespace PDDLTools.ErrorList
{
    [Export(typeof(ITextViewConnectionListener))]
    [ContentType(Constants.PDDLLanguageName)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class PDDLQuickInfoSourceConnectionListener : ITextViewConnectionListener
    {
        public void SubjectBuffersConnected(ITextView textView, ConnectionReason reason, IReadOnlyCollection<ITextBuffer> subjectBuffers)
        {
            if (PDDLQuickInfoData.QuickInfoContent.Count == 0)
                PDDLQuickInfoData.InitializeInfo();
        }

        public void SubjectBuffersDisconnected(ITextView textView, ConnectionReason reason, IReadOnlyCollection<ITextBuffer> subjectBuffers)
        {

        }
    }
}
