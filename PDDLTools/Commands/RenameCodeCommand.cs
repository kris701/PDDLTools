using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using PDDLTools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Text.Operations;
using PDDLTools.ContextStorage;
using PDDLParser.Helpers;
using PDDLParser.Models;
using PDDLTools.Windows.RenameCodeWindow;
using static PDDLTools.Windows.RenameCodeWindow.RenameCodeWindowControl;

namespace PDDLTools.Commands
{
    internal sealed class RenameCodeCommand : BaseCommand
    {
        public override int CommandId { get; } = 277;
        public static RenameCodeCommand Instance { get; internal set; }

        private RenameCodeCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, false)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new RenameCodeCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var textManager = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
            IVsTextView activeView = null;
            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out activeView));
            var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            var textView = editorAdapter.GetWpfTextView(activeView);
            var document = (textView.TextBuffer.ContentType.TypeName.Equals(Constants.PDDLLanguageName)) ? textView : null;
            if (document != null)
            {
                var word = GetSelectedWord(document.Caret.Position, document.TextBuffer);
                if (word != null)
                {
                    var currentFile = await DTE2Helper.GetSourceFilePathAsync();
                    if (PDDLHelper.IsFileDomain(currentFile))
                    {
                        var domainContext = PDDLFileContexts.GetDomainContextForFile(currentFile);
                        var node = GetValidNodeFromWord(domainContext, document, word.Value.GetText());
                        if (node != null)
                        {
                            ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(RenameCodeWindow), 0, true, this.package.DisposalToken);
                            if ((null == window) || (null == window.Frame))
                            {
                                throw new NotSupportedException("Cannot create tool window");
                            }
                            if (window.Content is RenameCodeWindowControl control)
                                await control.UpdateReplaceDataAsync(node, word.Value.GetText(), GetReplaceTypeFromNodeContext(node));
                        }
                    } 
                    else if (PDDLHelper.IsFileProblem(currentFile))
                    {
                        var problemContext = PDDLFileContexts.GetProblemContextForFile(currentFile);
                        var node = GetValidNodeFromWord(problemContext, document, word.Value.GetText());
                        if (node != null)
                        {
                            ToolWindowPane window = await this.package.ShowToolWindowAsync(typeof(RenameCodeWindow), 0, true, this.package.DisposalToken);
                            if ((null == window) || (null == window.Frame))
                            {
                                throw new NotSupportedException("Cannot create tool window");
                            }
                            if (window.Content is RenameCodeWindowControl control)
                                await control.UpdateReplaceDataAsync(node, word.Value.GetText(), GetReplaceTypeFromNodeContext(node));
                        }
                    }
                }
            }
        }

        private SnapshotSpan? GetSelectedWord(CaretPosition caretPosition, ITextBuffer buffer)
        {
            SnapshotPoint? point = caretPosition.Point.GetPoint(buffer, caretPosition.Affinity);

            if (point != null)
            {
                var word = TaggerHelper.GetExtendOfObjectWord(point.Value);
                if (word == null)
                    return null;
                bool foundWord = true;
                if (!WordExtentIsValid(point.Value, word.Value))
                {
                    if (word.Value.Span.Start != point.Value
                         || point.Value == point.Value.GetContainingLine().Start
                         || char.IsWhiteSpace((point.Value - 1).GetChar()))
                    {
                        foundWord = false;
                    }
                    else
                    {
                        word = TaggerHelper.GetExtendOfObjectWord(point.Value - 1);

                        if (!WordExtentIsValid(point.Value, word.Value))
                            foundWord = false;
                    }
                }

                if (!foundWord)
                    return null;

                SnapshotSpan currentWord = word.Value.Span;
                return currentWord;
            }
            return null;
        }

        static bool WordExtentIsValid(SnapshotPoint currentRequest, TextExtent word)
        {
            return word.IsSignificant
                && currentRequest.Snapshot.GetText(word.Span).Any(c => char.IsLetter(c));
        }

        private INode GetValidNodeFromWord(INode source, IWpfTextView document, string word)
        {
            var possibleNodes = source.FindName(word);
            if (possibleNodes.Count > 0)
            {
                int simpleCursorPosition = document.Caret.Position.BufferPosition.Position;
                INode targetNode = possibleNodes.First();
                int shortestDist = Math.Abs(targetNode.Start - simpleCursorPosition);
                foreach (var node in possibleNodes.Skip(1))
                {
                    var dist = Math.Abs(node.Start - simpleCursorPosition);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        targetNode = node;
                    }
                }

                if (targetNode is PredicateExp || targetNode is NameExp)
                {
                    return targetNode;
                }
            }
            return null;
        }

        private ReplaceTypes GetReplaceTypeFromNodeContext(INode node)
        {
            if (node is PredicateExp)
                return ReplaceTypes.Predicate;
            if (node is NameExp name)
            {

            }

            return ReplaceTypes.None;
        }
    }
}
