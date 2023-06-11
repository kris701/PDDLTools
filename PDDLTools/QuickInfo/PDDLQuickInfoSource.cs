using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using PDDLTools.Options;
using PDDLTools.QuickInfo.PDDLInfo;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace PDDLTools.QuickInfo
{
    internal class PDDLQuickInfoSource : IAsyncQuickInfoSource
    {
        private ITextBuffer _textBuffer;
        private PDDLQuickInfoSourceProvider _toolTipProvider;

        public PDDLQuickInfoSource(ITextBuffer subjectBuffer, PDDLQuickInfoSourceProvider toolTipProvider)
        {
            _textBuffer = subjectBuffer;
            _toolTipProvider = toolTipProvider;
        }

        private bool m_isDisposed;
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                GC.SuppressFinalize(this);
                m_isDisposed = true;
            }
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            SnapshotPoint? subjectTriggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);
            if (subjectTriggerPoint != null)
            {
                Task<QuickInfoItem> t = new Task<QuickInfoItem>(() =>
                {
                    ITextSnapshotLine line = subjectTriggerPoint.Value.GetContainingLine();
                    ITrackingSpan lineSpan = _textBuffer.CurrentSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeInclusive);

                    if (PDDLInfo.PDDLInfo.QuickInfoContent.Count == 0)
                        return new QuickInfoItem(lineSpan, "Quickinfo is loading...");
                    else
                    {
                        string lineText = line.GetText();
                        string hoverText = GetWordUnderCursor(subjectTriggerPoint);

                        foreach (string key in PDDLInfo.PDDLInfo.QuickInfoContent.Keys.OrderByDescending(x => x.Length))
                            if (lineText.Contains(key))
                                if (key.Contains(hoverText))
                                    return new QuickInfoItem(lineSpan, PDDLInfo.PDDLInfo.QuickInfoContent[key]);
                        return new QuickInfoItem(null, "");
                    }
                });
                t.Start();
                return t;
            }

            return Task.FromResult<QuickInfoItem>(null);
        }

        private string GetWordUnderCursor(SnapshotPoint? subjectTriggerPoint)
        {
            SnapshotSpan querySpan = new SnapshotSpan(subjectTriggerPoint.Value, 0);
            ITextStructureNavigator navigator = _toolTipProvider.NavigatorService.GetTextStructureNavigator(_textBuffer);
            var test = navigator.GetSpanOfEnclosing(querySpan);
            TextExtent extent = navigator.GetExtentOfWord(subjectTriggerPoint.Value);
            string searchText = extent.Span.GetText();
            return searchText;
        }
    }
}
