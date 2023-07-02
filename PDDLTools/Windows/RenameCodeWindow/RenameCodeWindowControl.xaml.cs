using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.VCProjectEngine;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
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
            if (PDDLInfo.PDDLInfo.PDDLDefinition != null)
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
                    newTip.Content = "Invaid replacement name! Might conflict with other names!";
                    newTip.IsOpen = true;
                    ReplaceWithTextbox.ToolTip = newTip;
                }
            }
        }

        private async Task StartReplacingAsync()
        {
            var doc = await DTE2Helper.GetSourceFilePathAsync();
            var text = File.ReadAllText(doc);
            var newText = ReplaceNamesOfType(text, ReplaceTextTextbox.Text, ReplaceWithTextbox.Text, _scopeType);
            File.WriteAllText(doc, newText);

            await HideThisWindowAsync();
        }

        private string ReplaceNamesOfType(string text, string from, string to, ReplaceScopeTypes scopeType)
        {
            List<INode> nodes = new List<INode>();

            switch (scopeType)
            {
                case ReplaceScopeTypes.ActionParameter:
                    nodes = GetActionFromScope(_node).FindNames(from).ToList(); break;
                case ReplaceScopeTypes.AxiomParameter:
                    nodes = GetAxiomFromScope(_node).FindNames(from).ToList(); break;
                case ReplaceScopeTypes.ActionName:
                case ReplaceScopeTypes.Predicate:
                case ReplaceScopeTypes.ProblemObjects:
                case ReplaceScopeTypes.TypeName:
                    nodes = GetFullScope(_node).FindNames(from).ToList(); 
                    break;
            }

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
            return text;
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

        private INode GetFullScope(INode node)
        {
            if (node.Parent == null)
                return node;
            return GetFullScope(node.Parent);
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
