using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using PDDLParser;
using PDDLParser.Exceptions;
using PDDLParser.Helpers;
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

            try
            {
                if (PDDLHelper.IsFileDomain(file))
                {
                    DomainPanel.Visibility = Visibility.Visible;

                    IPDDLParser parser = new PDDLParser.PDDLParser();
                    var domain = parser.ParseDomainFile(file);

                    if (domain.Predicates != null)
                        PredicateCountLabel.Content = $"{domain.Predicates.Predicates.Count}";
                    if (domain.Actions != null)
                        ActionCountLabel.Content = $"{domain.Actions.Count}";
                }
                else if (PDDLHelper.IsFileProblem(file))
                {
                    ProblemPanel.Visibility = Visibility.Visible;

                    IPDDLParser parser = new PDDLParser.PDDLParser();
                    var problem = parser.ParseProblemFile(file);

                    if (problem.Objects != null)
                        ObjectCountLabel.Content = $"{problem.Objects.Objs.Count}";
                    if (problem.Init != null)
                        InitCountLabel.Content = $"{problem.Init.Predicates.Count}";
                    if (problem.Goal != null)
                        GoalCountLabel.Content = $"{problem.Goal.GoalExpCount}";
                }
            }
            catch (Exception) { }
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
