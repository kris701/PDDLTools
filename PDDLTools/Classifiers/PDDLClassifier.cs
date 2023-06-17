using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Classifiers
{ 
    class PDDLClassifier : IClassifier
    {
        internal Dictionary<string, List<string>> ClassificationStorage = new Dictionary<string, List<string>>()
        {
            { PDDLTypes.Declaration, new List<string>(){ "define", "domain", "problem" } },
            { PDDLTypes.Type, new List<string>(){ ":extends", ":requirements", ":types", ":constants", ":predicates", ":timeless", ":action", ":axiom", ":domain", ":objects", ":init", ":goal" } },
            { PDDLTypes.Expression, new List<string>(){ "and", "or", "not", "=" } },
            { PDDLTypes.Parameter, new List<string>(){ ":parameters", ":precondition", ":effect", ":vars", ":context", ":implies" } },
            { PDDLTypes.Externals, new List<string>(){ ":strips", ":typing" } },
            { PDDLTypes.MinorToken, new List<string>(){ "(", ")", " - ", "?" } },
        };

        internal List<string> IgnoreLineTokens = new List<string>()
        {
            ";"
        };

        ITextBuffer _buffer;
        IClassificationTypeRegistryService _typeRegistry;

        public PDDLClassifier(ITextBuffer buffer, IClassificationTypeRegistryService typeRegistry)
        {
            _buffer = buffer;
            _typeRegistry = typeRegistry;

            _buffer.Changed += OnBufferChanged;
        }

        void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var temp = ClassificationChanged;
            if (temp != null)
                foreach (var change in e.Changes)
                    temp(this, new ClassificationChangedEventArgs(new SnapshotSpan(e.After, change.NewSpan)));
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;

            List<ClassificationSpan> classificationSpans = new List<ClassificationSpan>();

            foreach(string classifications in ClassificationStorage.Keys)
            {
                IClassificationType classificationType = _typeRegistry.GetClassificationType(classifications);

                ITextSnapshotLine startLine = span.Start.GetContainingLine();
                int startLineNumber = startLine.LineNumber;
                int endLineNumber = (span.End <= startLine.End) ? startLineNumber : snapshot.GetLineNumberFromPosition(span.End);

                for (int lineNumber = startLineNumber; lineNumber <= endLineNumber; lineNumber++)
                {
                    ITextSnapshotLine line = snapshot.GetLineFromLineNumber(lineNumber);

                    string text = line.GetText().ToLower();
                    if (IgnoreLineTokens.Any(x => text.Contains(x)))
                        continue;
                    foreach(var match in ClassificationStorage[classifications].OrderBy(x => x.Length))
                    {
                        if (text.Contains(match))
                        {
                            bool any = false;
                            for (int i = 0; i < text.Length - match.Length + 1; i++)
                            {
                                if (text.Substring(i, match.Length) == match)
                                {
                                    if (match.Any(x => char.IsLetter(x)))
                                        if (!IsIsolatedToken(text, i, match.Length))
                                            continue;
                                    any = true;
                                    SnapshotSpan matchedSpan = new SnapshotSpan(line.Start + i, match.Length);
                                    classificationSpans.Add(new ClassificationSpan(matchedSpan, classificationType));
                                }
                            }

                            if (any)
                                text = text.Replace(match, new string(' ', match.Length));
                        }
                    }
                }
            }

            return classificationSpans;
        }

        private bool IsIsolatedToken(string text, int index, int length)
        {
            if (index > 0)
            {
                if (char.IsLetter(text[index - 1]))
                    return false;
            }
            if (index + length < text.Length)
            {
                if (char.IsLetter(text[index + length]))
                    return false;
            }
            return true;
        }
    }
}
