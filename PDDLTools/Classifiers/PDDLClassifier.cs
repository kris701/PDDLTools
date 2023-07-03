using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLTools.ContextStorage;
using PDDLTools.Helpers;
using PDDLParser.Helpers;
using System.Runtime.Remoting.Contexts;

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
            { PDDLTypes.Predicates, new List<string>() },
            { PDDLTypes.Objects, new List<string>() },
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

        private void SetupDynamicStorage()
        {
            ClassificationStorage[PDDLTypes.Predicates] = new List<string>();
            ClassificationStorage[PDDLTypes.Objects] = new List<string>();

            var file = DTE2Helper.GetSourceFilePathAsync().Result;
            var decl = PDDLFileContexts.TryGetContextForFile(file);
            if (decl != null)
            {
                if (decl.Domain != null)
                {
                    if (decl.Domain.Predicates != null)
                        foreach (var pred in decl.Domain.Predicates.Predicates)
                            ClassificationStorage[PDDLTypes.Predicates].Add(pred.Name);
                }
                else if (decl.Problem != null)
                {
                    if (decl.Problem.Init != null)
                        foreach (var pred in decl.Problem.Init.Predicates)
                            ClassificationStorage[PDDLTypes.Predicates].Add(pred.Name);
                    if (decl.Problem.Objects != null)
                        foreach (var obj in decl.Problem.Objects.Objs)
                            ClassificationStorage[PDDLTypes.Objects].Add(obj.Name);
                }
            }
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            ITextSnapshot snapshot = span.Snapshot;

            List<ClassificationSpan> classificationSpans = new List<ClassificationSpan>();

            SetupDynamicStorage();

            foreach (string classifications in ClassificationStorage.Keys)
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

                    foreach (var match in ClassificationStorage[classifications].OrderBy(x => x.Length))
                    {
                        if (text.Contains(match))
                        {
                            for (int i = 0; i < text.Length - match.Length + 1; i++)
                            {
                                if (text.Substring(i, match.Length) == match)
                                {
                                    if (match.Any(x => char.IsLetter(x)))
                                    {
                                        if (IsIsolatedToken(text, i, match.Length))
                                        {
                                            SnapshotSpan matchedSpan = new SnapshotSpan(line.Start + i, match.Length);
                                            classificationSpans.Add(new ClassificationSpan(matchedSpan, classificationType));
                                            text = ReplaceRangeWith(text, line.Start + i, line.Start + i + match.Length, ' ');
                                        }
                                    }
                                    else
                                    {
                                        SnapshotSpan matchedSpan = new SnapshotSpan(line.Start + i, match.Length);
                                        classificationSpans.Add(new ClassificationSpan(matchedSpan, classificationType));
                                        text = ReplaceRangeWith(text, line.Start + i, line.Start + i + match.Length, ' ');
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return classificationSpans;
        }

        private string ReplaceRangeWith(string text, int start, int end, char with)
        {
            var newStr = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (i < start || i > end)
                    newStr += text[i];
                else
                    newStr += with;
            }
            return text;
        }

        private bool IsIsolatedToken(string text, int index, int length)
        {
            if (index > 0)
            {
                if (char.IsLetter(text[index - 1]) 
                    || char.IsNumber(text[index - 1])
                    || text[index - 1] == '-' 
                    || text[index - 1] == '?')
                    return false;
            }
            if (index + length < text.Length)
            {
                if (char.IsLetter(text[index + length]) 
                    || char.IsNumber(text[index + length]) 
                    || text[index + length] == '-' 
                    || text[index + length] == '?')
                    return false;
            }
            return true;
        }
    }
}
