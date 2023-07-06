using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VCProjectEngine;
using PDDLParser.Helpers;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLTools.ContextStorage;
using PDDLTools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static PDDLTools.Windows.RenameCodeWindow.RenameCodeWindowControl;

namespace PDDLTools.Windows.RenameCodeWindow
{
    public partial class RenameCodeWindowControl : UserControl
    {
        public enum ReplaceScopeTypes { None, Predicate, ActionParameter, AxiomParameter, ActionName, ProblemObjects, TypeName };
        private PDDLToolsPackage _package;
        private INode _node;
        private ReplaceScopeTypes _scopeType;
        private bool _isLoaded = false;

        public RenameCodeWindowControl()
        {
            InitializeComponent();
        }

        public async Task UpdateReplaceDataAsync(PDDLToolsPackage package, INode node, string text, ReplaceScopeTypes scopeType)
        {
            while (!_isLoaded)
                await Task.Delay(100);
            _package = package;
            _node = node;
            ReplaceTextTextbox.Text = text;
            ReplaceWithTextbox.Text = "";
            ReplaceWithBorder.BorderThickness = new Thickness(0);
            _scopeType = scopeType;
        }

        private async void ReplaceButton_Click(object sender, RoutedEventArgs e)
        {
            await CheckStartReplaingAsync();
        }

        private bool IsReplacementValid(string text)
        {
            if (text == "")
                return false;
            else if (text.Contains(":"))
                return false;
            else if (PDDLInfo.PDDLInfo.PDDLDefinition != null)
            {
                foreach (var element in PDDLInfo.PDDLInfo.PDDLDefinition.Elements)
                    if (element.Key == text)
                        return false;
            }
            return true;
        }

        private async Task CheckStartReplaingAsync()
        {
            if (ReplaceTextTextbox.Text == ReplaceWithTextbox.Text)
                await HideThisWindowAsync();
            else
            {
                if (IsReplacementValid(ReplaceWithTextbox.Text))
                    await StartReplacingAsync();
                else
                {
                    ReplaceWithBorder.BorderThickness = new Thickness(1);
                    var newTip = new ToolTip();
                    newTip.Content = "Invaid replacement name! Might conflict with other names or default PDDL names!";
                    newTip.IsOpen = true;
                    ReplaceWithTextbox.ToolTip = newTip;
                }
            }
        }

        private async Task StartReplacingAsync()
        {
            if (IsGlobalRenameValid(_scopeType))
            {
                string targetDomain = null;
                var sourceFile = await DTE2Helper.GetSourceFilePathAsync();
                var decl = PDDLFileContexts.TryGetContextForFile(sourceFile);
                if (decl != null)
                {
                    if (decl.Domain != null)
                        targetDomain = decl.Domain.Name.Name;
                    else if (decl.Problem != null)
                        targetDomain = decl.Problem.DomainName.Name;
                }
                if (targetDomain != null)
                {
                    List<string> targetFiles = new List<string>() { sourceFile };
                    foreach (var file in Directory.GetFiles(new FileInfo(sourceFile).Directory.FullName))
                    {
                        if (targetFiles.Contains(file))
                            continue;
                        var otherDecl = PDDLFileContexts.TryGetContextForFile(file);
                        if (otherDecl != null)
                        {
                            if (otherDecl.Domain != null)
                            {
                                if (otherDecl.Domain.Name.Name == targetDomain)
                                    targetFiles.Add(file);
                            }
                            else if (otherDecl.Problem != null)
                            {
                                if (otherDecl.Problem.DomainName.Name == targetDomain)
                                    targetFiles.Add(file);
                            }
                        } 
                    }

                    foreach (var file in targetFiles)
                    {
                        var context = PDDLFileContexts.TryGetContextForFile(file);
                        if (context != null)
                            ReplaceNamesOfType(
                                file,
                                context.FindNames(ReplaceTextTextbox.Text).ToList(),
                                ReplaceTextTextbox.Text,
                                ReplaceWithTextbox.Text,
                                _scopeType);
                    }
                }
            }
            else
            {
                var file = await DTE2Helper.GetSourceFilePathAsync();
                List<INamedNode> nodes = new List<INamedNode>();
                switch (_scopeType)
                {
                    case ReplaceScopeTypes.ActionParameter:
                        nodes = GetActionFromScope(_node).FindNames(ReplaceTextTextbox.Text).ToList(); break;
                    case ReplaceScopeTypes.AxiomParameter:
                        nodes = GetAxiomFromScope(_node).FindNames(ReplaceTextTextbox.Text).ToList(); break;
                    case ReplaceScopeTypes.ActionName:
                    case ReplaceScopeTypes.Predicate:
                    case ReplaceScopeTypes.ProblemObjects:
                    case ReplaceScopeTypes.TypeName:
                        var context = PDDLFileContexts.TryGetContextForFile(file);
                        if (context != null)
                            nodes = context.FindNames(ReplaceTextTextbox.Text).ToList();
                        break;
                }
                if (nodes.Count > 0)
                    ReplaceNamesOfType(file, nodes, ReplaceTextTextbox.Text, ReplaceWithTextbox.Text, _scopeType);
            }

            await HideThisWindowAsync();
        }

        private bool IsGlobalRenameValid(ReplaceScopeTypes scopeType)
        {
            if (IsGlobalRename.IsChecked == false)
                return false;
            else if (IsGlobalRename.IsChecked == true)
            {
                if (scopeType == ReplaceScopeTypes.ActionParameter || scopeType == ReplaceScopeTypes.AxiomParameter)
                    return false;
            }
            return true;
        }

        private void ReplaceNamesOfType(string file, List<INamedNode> nodes, string from, string to, ReplaceScopeTypes scopeType)
        {
            var text = File.ReadAllText(file);

            int offset = 0;
            foreach(var node in nodes)
            {
                switch (scopeType)
                {
                    case ReplaceScopeTypes.ActionParameter:
                    case ReplaceScopeTypes.AxiomParameter:
                    case ReplaceScopeTypes.ProblemObjects:
                        if (node is NameExp)
                        {
                            text = ReplaceTextWithinNode(text, from, to, node, offset);
                            offset += (to.Length - from.Length);
                        }
                        break;
                    case ReplaceScopeTypes.Predicate:
                        if (node is PredicateExp)
                        {
                            text = ReplaceTextWithinNode(text, from, to, node, offset);
                            offset += (to.Length - from.Length);
                        }
                        break;
                    case ReplaceScopeTypes.ActionName:
                        if (node is ActionDecl)
                        {
                            text = ReplaceTextWithinNode(text, from, to, node, offset);
                            offset += (to.Length - from.Length);
                        }
                        break;
                    case ReplaceScopeTypes.TypeName:
                        if (node is TypeNameDecl)
                        {
                            text = ReplaceTextWithinNode(text, from, to, node, offset);
                            offset += (to.Length - from.Length);
                        }
                        break;
                }
            }

            File.WriteAllText(file, text);
        }

        private string ReplaceTextWithinNode(string text, string from, string to, INode node, int offset)
        {
            var indexOf = text.IndexOf(from, node.Start + offset);
            if (indexOf >= node.Start + offset && indexOf <= node.End + offset)
                text = text.Remove(indexOf, from.Length).Insert(indexOf, to);
            return text;
        }

        private INode GetActionFromScope(INode exp)
        {
            if (exp.Parent is null)
                return null;
            if (exp.Parent is ActionDecl act)
                return act;
            return GetActionFromScope(exp.Parent);
        }

        private INode GetAxiomFromScope(INode exp)
        {
            if (exp.Parent is null)
                return null;
            if (exp.Parent is AxiomDecl act)
                return act;
            return GetAxiomFromScope(exp.Parent);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private async Task HideThisWindowAsync()
        {
            var window = await _package.FindToolWindowAsync(typeof(RenameCodeWindow), 0, false, _package.DisposalToken);
            if (window.Frame is IVsWindowFrame frame)
                frame.Hide();
        }

        private async void ReplaceWithTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                await CheckStartReplaingAsync();
        }

        private async void ReplaceWithTextbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                await HideThisWindowAsync();
        }
    }
}
