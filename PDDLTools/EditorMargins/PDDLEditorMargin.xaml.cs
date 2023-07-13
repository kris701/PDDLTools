using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using PDDLParser;
using PDDLParser.Exceptions;
using PDDLParser.Helpers;
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

namespace PDDLTools.EditorMargins
{
    public partial class PDDLEditorMargin : UserControl, IWpfTextViewMargin
    {
        public const string MarginName = "PDDL Editor Margin";

        private bool isDisposed;

        List<object> events = new List<object>();

        public PDDLEditorMargin(IWpfTextView textView)
        {
            InitializeComponent();

            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = DTE2Helper.GetDTE2();
            var docEvent = dte2.Events.DocumentEvents;
            events.Add(docEvent);
            docEvent.DocumentSaved += CheckDocument;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await SetupMarginAsync();
        }

        public async void CheckDocument(EnvDTE.Document document)
        {
            await SetupMarginAsync();
        }

        private async Task SetupMarginAsync()
        {
            if (await DTE2Helper.IsValidFileOpenAsync())
            {
                var file = await DTE2Helper.GetSourceFilePathAsync();
                while (file == null)
                {
                    await Task.Delay(1000);
                    file = await DTE2Helper.GetSourceFilePathAsync();
                }

                PredicateCountLabel.Content = "?";
                ActionCountLabel.Content = "?";
                ObjectCountLabel.Content = "?";
                InitCountLabel.Content = "?";
                GoalCountLabel.Content = "?";

                DomainPanel.Visibility = Visibility.Hidden;
                ProblemPanel.Visibility = Visibility.Hidden;

                var decl = await PDDLFileContexts.TryGetContextForFileAsync(file);
                if (decl  != null)
                {
                    if (decl.Domain != null)
                    {
                        DomainPanel.Visibility = Visibility.Visible;
                        if (decl.Domain.Predicates != null)
                            PredicateCountLabel.Content = $"{decl.Domain.Predicates.Predicates.Count}";
                        if (decl.Domain.Actions != null)
                            ActionCountLabel.Content = $"{decl.Domain.Actions.Count}";
                    }
                    else if (decl.Problem != null)
                    {
                        ProblemPanel.Visibility = Visibility.Visible;

                        if (decl.Problem.Objects != null)
                            ObjectCountLabel.Content = $"{decl.Problem.Objects.Objs.Count}";
                        if (decl.Problem.Init != null)
                            InitCountLabel.Content = $"{decl.Problem.Init.Predicates.Count}";
                        if (decl.Problem.Goal != null)
                            GoalCountLabel.Content = $"{decl.Problem.Goal.PredicateCount}";
                    }
                }
            }
        }

        #region IWpfTextViewMargin

        public FrameworkElement VisualElement
        {
            get
            {
                this.ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin

        public double MarginSize
        {
            get
            {
                this.ThrowIfDisposed();

                return this.ActualHeight;
            }
        }

        public bool Enabled
        {
            get
            {
                this.ThrowIfDisposed();
                return true;
            }
        }
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, PDDLEditorMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                GC.SuppressFinalize(this);
                this.isDisposed = true;
            }
        }

        #endregion

        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }
    }
}
