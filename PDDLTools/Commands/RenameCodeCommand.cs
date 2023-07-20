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
using PDDLParser.Models.Domain;
using PDDLParser;
using PDDLParser.Models.Problem;
using System.ComponentModel.Design;
using PDDLTools.Windows.PlanValidatorWindow;

namespace PDDLTools.Commands
{
    internal sealed class RenameCodeCommand : BaseCommand<RenameCodeCommand>
    {
        public override int CommandId { get; } = 277;

        private RenameCodeCommand(AsyncPackage package, OleMenuCommandService commandService) : base(package, commandService, true)
        {
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new RenameCodeCommand(package, await InitializeCommandServiceAsync(package));
        }

        public override async void CheckQueryStatus(object sender, EventArgs e)
        {
            if (sender is MenuCommand button)
            {
                button.Visible = false;
                var document = GetDocument();
                if (document == null)
                    return;
                var selectedWord = GetSelectedWord(document);
                if (selectedWord == null)
                    return;
                var selectedNode = await GetSelectedAsNodeAsync(selectedWord.ToLower(), document.Caret.Position.BufferPosition.Position);
                if (selectedNode == null)
                    return;
                button.Visible = true;
            }
        }

        public override async Task ExecuteAsync(object sender, EventArgs e)
        {
            var document = GetDocument();
            if (document == null)
                return;
            var selectedWord = GetSelectedWord(document);
            if (selectedWord == null)
                return;
            var selectedNode = await GetSelectedAsNodeAsync(selectedWord.ToLower(), document.Caret.Position.BufferPosition.Position);
            if (selectedNode == null)
                return;

            var scope = GetReplaceTypeFromNodeContext(selectedNode);
            if (scope != ReplaceScopeTypes.None)
                await CreateWindowAsync(selectedNode, selectedWord, scope);
        }

        private IWpfTextView GetDocument()
        {
            var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
            var textManager = (IVsTextManager)Package.GetGlobalService(typeof(SVsTextManager));
            IVsTextView activeView = null;
            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out activeView));
            var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            var textView = editorAdapter.GetWpfTextView(activeView);
            var document = (textView.TextBuffer.ContentType.TypeName.Equals(Constants.PDDLLanguageName)) ? textView : null;
            return document;
        }

        private string GetSelectedWord(IWpfTextView document)
        {
            var word = GetSelectedWord(document.Caret.Position, document.TextBuffer);
            if (word != null)
                return word.Value.GetText();
            return null;
        }

        private async Task<INode> GetSelectedAsNodeAsync(string word, int atIndex)
        {
            var currentFile = await DTE2Helper.GetSourceFilePathAsync();
            var decl = await PDDLFileContexts.TryGetContextForFileAsync(currentFile);
            if (decl != null)
            {
                if (decl.Domain != null)
                    return GetValidNodeFromWord(decl.Domain, atIndex, word);
                if (decl.Problem != null)
                    return GetValidNodeFromWord(decl.Problem, atIndex, word);
            }
            return null;
        }

        private async Task CreateWindowAsync(INode node, string word, ReplaceScopeTypes scope)
        {
            var window = await OpenWindowOfTypeAsync(typeof(RenameCodeWindow));
            if (window.Content is RenameCodeWindowControl control)
                await control.UpdateReplaceDataAsync(this._package as PDDLToolsPackage, node, word, scope);
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

        private INode GetValidNodeFromWord(INode source, int cursor, string word)
        {
            var possibleNodes = source.FindNames(word);
            if (possibleNodes.Count > 0)
            {
                INode targetNode = possibleNodes.First();
                int shortestDist = Math.Abs(targetNode.Start - cursor);
                foreach (var node in possibleNodes.Skip(1))
                {
                    var dist = Math.Abs(node.Start - cursor);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        targetNode = node;
                    }
                }

                if (targetNode is PredicateExp || targetNode is NameExp)
                    return targetNode;
                if (targetNode is ActionDecl act)
                {
                    if (word == act.Name)
                        return targetNode;
                }

            }
            return null;
        }

        private ReplaceScopeTypes GetReplaceTypeFromNodeContext(INode node)
        {
            if (node is PredicateExp)
                return ReplaceScopeTypes.Predicate;
            else if (node is ActionDecl)
                return ReplaceScopeTypes.ActionName;
            else if (node is NameExp name)
            {
                if (IsActionScope(name))
                    return ReplaceScopeTypes.ActionParameter;
                else if (IsAxiomScope(name))
                    return ReplaceScopeTypes.AxiomParameter;
                else if (IsObjectScope(name))
                    return ReplaceScopeTypes.ProblemObjects;
                else return ReplaceScopeTypes.TypeName;
            }
            return ReplaceScopeTypes.None;
        }

        private bool IsActionScope(INode exp)
        {
            if (exp.Parent == null)
                return false;
            if (exp.Parent is ActionDecl)
                return true;
            return IsActionScope(exp.Parent);
        }

        private bool IsAxiomScope(INode exp)
        {
            if (exp.Parent == null)
                return false;
            if (exp.Parent is AxiomDecl)
                return true;
            return IsAxiomScope(exp.Parent);
        }

        private bool IsObjectScope(INode exp)
        {
            if (exp.Parent == null)
                return false;
            if (exp.Parent is ObjectsDecl)
                return true;
            return IsObjectScope(exp.Parent);
        }
    }
}
