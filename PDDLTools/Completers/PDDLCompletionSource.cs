using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using PDDLParser.Helpers;
using PDDLParser.Models;
using PDDLTools.ContextStorage;
using PDDLTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Completers
{
    // https://learn.microsoft.com/en-us/visualstudio/extensibility/walkthrough-displaying-statement-completion?view=vs-2022&tabs=csharp
    internal class PDDLCompletionSource : ICompletionSource
    {
        private PDDLCompletionSourceProvider m_sourceProvider;
        private ITextBuffer m_textBuffer;
        private List<Completion> m_compList;

        public PDDLCompletionSource(PDDLCompletionSourceProvider sourceProvider, ITextBuffer textBuffer)
        {
            m_sourceProvider = sourceProvider;
            m_textBuffer = textBuffer;
        }

        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            if (PDDLInfo.PDDLInfo.PDDLDefinition == null)
                PDDLInfo.PDDLInfo.InitializeInfo();

            List<string> strList = new List<string>();
            foreach (var element in PDDLInfo.PDDLInfo.PDDLDefinition.Elements)
                strList.Add(element.Key);

            m_compList = new List<Completion>();
            foreach (string str in strList)
                m_compList.Add(new Completion(str, str, str, null, null));

            var currentFile = DTE2Helper.GetSourceFilePathAsync().Result;
            var decl = PDDLFileContexts.TryGetContextForFile(currentFile);
            if (decl != null)
            {
                if (decl.Domain != null)
                {
                    if (decl.Domain.Predicates != null)
                        foreach (var predicate in decl.Domain.Predicates.Predicates)
                            m_compList.Add(new Completion(predicate.Name, predicate.Name, predicate.Name, null, null));
                }
                else if (decl.Problem != null)
                {
                    if (decl.Problem.Objects != null)
                        foreach (var obj in decl.Problem.Objects.Objs)
                            m_compList.Add(new Completion(obj.Name, obj.Name, obj.Name, null, null));
                    if (decl.Problem.Init != null)
                        foreach (var pred in decl.Problem.Init.Predicates)
                            if (pred is INamedNode node)
                                m_compList.Add(new Completion(node.Name, node.Name, node.Name, null, null));
                }
            }

            completionSets.Add(new CompletionSet(
                "Tokens",    //the non-localized title of the tab
                "Tokens",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(m_textBuffer),
                    session),
                m_compList,
                null));
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = m_sourceProvider.NavigatorService.GetTextStructureNavigator(m_textBuffer);
            var extent = TaggerHelper.GetExtendOfObjectWord(currentPoint);
            if (extent == null)
                return currentPoint.Snapshot.CreateTrackingSpan(new Span(), SpanTrackingMode.EdgeInclusive);
            return currentPoint.Snapshot.CreateTrackingSpan(extent.Value.Span, SpanTrackingMode.EdgeInclusive);
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
    }
}
