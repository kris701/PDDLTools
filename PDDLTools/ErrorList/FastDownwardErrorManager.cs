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
using HaskellTools.Helpers;
using PDDLParser;
using PDDLParser.Exceptions;
using Microsoft.Build.Framework.XamlTypes;
using Microsoft.VisualStudio.Text.Classification;
using System.Windows.Shapes;
using PDDLParser.Listener;
using static System.Windows.Forms.LinkLabel;

namespace PDDLTools.ErrorList
{
    internal class FastDownwardErrorManager
    {
        public static FastDownwardErrorManager Instance;
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

            var file = await DTE2Helper.GetSourceFilePathAsync();
            var parser = new PDDLParser.PDDLParser();
            try
            {   if (PDDLHelper.IsFileDomain(file))
                {
                    var fullDomain = parser.ParseDomainFile(file);
                } 
                else if (PDDLHelper.IsFileProblem(file))
                {
                    var fullProblem = parser.ParseProblemFile(file);
                }
            }
            catch (ParseException ex)
            {
                
            }

            if (parser.Listener.Errors.Count > 0)
            {
                var sourceDocumentLines = File.ReadAllLines(file);
                foreach (var error in parser.Listener.Errors)
                {
                    ErrorTask newError = new ErrorTask();

                    switch (error.Type)
                    {
                        case ParseErrorType.Error: newError.ErrorCategory = TaskErrorCategory.Error; break;
                        case ParseErrorType.Warning: newError.ErrorCategory = TaskErrorCategory.Warning; break;
                        case ParseErrorType.Message: newError.ErrorCategory = TaskErrorCategory.Message; break;
                    }
                    switch (error.Level)
                    {
                        case ParserErrorLevel.Low: newError.Priority = TaskPriority.Low; break;
                        case ParserErrorLevel.Medium: newError.Priority = TaskPriority.Normal; break;
                        case ParserErrorLevel.High: newError.Priority = TaskPriority.High; break;
                    }

                    newError.Text = error.Message;
                    newError.Line = error.Line - 1;
                    //newError.Column = error.Character;
                    newError.Column = GetColumnFromCharacter(sourceDocumentLines, error.Line, error.Character);
                    newError.Document = "";
                    newError.Navigate += JumpToError;
                    _errorProvider.Tasks.Add(newError);
                }
            }

            if (_errorProvider.Tasks.Count > 0)
                _errorProvider.Show();
        }

        private int GetColumnFromCharacter(string[] sourceDocumentLines, int lineNumber, int characterNumber)
        {
            int offset = 0;
            for (int i = 0; i < lineNumber - 1; i++)
            {
                if (i > sourceDocumentLines.Length)
                    break;
                offset += sourceDocumentLines[i].Length + 1;
            }
            return characterNumber - offset - 1;
        }

        private async void JumpToError(object sender, EventArgs e)
        {
            if (sender is ErrorTask item) {
                TextField.Caret.MoveTo(TextField.TextSnapshot.GetLineFromLineNumber(item.Line).Start + item.Column);
                TextField.Caret.EnsureVisible();
                await DTE2Helper.FocusActiveDocumentAsync();
            }
        }
    }
}
