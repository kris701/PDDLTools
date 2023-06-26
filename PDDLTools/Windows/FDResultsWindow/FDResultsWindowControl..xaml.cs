using CMDRunners.FastDownward;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PDDLTools.Windows.FDResultsWindow
{
    public partial class FDResultsWindowControl : UserControl
    {
        private FDResults _data;
        private bool _reDraw = false;
        public FDResultsWindowControl()
        {
            InitializeComponent();
        }

        public void SetupResultData(FDResults data)
        {
            _data = data;
            _reDraw = true;
        }

        private Grid GenerateLabel(string title, object content)
        {
            Grid returnGrid = new Grid();

            var col1 = new ColumnDefinition();
            col1.Width = new GridLength(20, GridUnitType.Star);
            var col2 = new ColumnDefinition();
            col2.Width = new GridLength(80, GridUnitType.Star);
            returnGrid.ColumnDefinitions.Add(col1);
            returnGrid.ColumnDefinitions.Add(col2);

            TextBox titleLabel = new TextBox();
            titleLabel.Text = title;
            titleLabel.IsReadOnly = true;
            titleLabel.Foreground = Brushes.White;
            titleLabel.Background = Brushes.Transparent;
            returnGrid.Children.Add(titleLabel);

            TextBox contentLabel = new TextBox();
            contentLabel.Text = content.ToString();
            contentLabel.IsReadOnly = true;
            contentLabel.Foreground = Brushes.White;
            contentLabel.Background = Brushes.Transparent;
            Grid.SetColumn(contentLabel, 1);
            returnGrid.Children.Add(contentLabel);

            return returnGrid;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            while (_data == null)
                await Task.Delay(100);

            if (_reDraw)
            {
                ResultsTitleLabel.Content = $"Fast Downward Run Results: {DateTime.Now}";

                GeneralExpanderPanel.Children.Clear();
                TranslatorExpanderPanel.Children.Clear();
                SearchExpanderPanel.Children.Clear();

                GeneralExpanderPanel.Children.Add(GenerateLabel("Solution found: ", _data.WasSolutionFound));
                GeneralExpanderPanel.Children.Add(GenerateLabel("Search Time (s): ", _data.SearchTime));
                GeneralExpanderPanel.Children.Add(GenerateLabel("Total Time (s): ", _data.TotalTime));
                GeneralExpanderPanel.Children.Add(GenerateLabel("Fast Downward Exit Code: ", Enum.GetName(typeof(FDExitCode), _data.ExitCode)));

                TranslatorExpanderPanel.Children.Add(GenerateLabel("Variables: ", _data.TranslatorVariables));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Derived Variables: ", _data.TranslatorDerivedVariables));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Facts: ", _data.TranslatorFacts));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Goal Facts: ", _data.TranslatorGoalFacts));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("MutexGroups: ", _data.TranslatorMutexGroups));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Total Mutex Groups Size: ", _data.TranslatorTotalMutexGroupsSize));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Operators: ", _data.TranslatorOperators));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Axioms: ", _data.TranslatorAxioms));
                TranslatorExpanderPanel.Children.Add(GenerateLabel("Task Size: ", _data.TranslatorTaskSize));

                SearchExpanderPanel.Children.Add(GenerateLabel("Plan Length: ", _data.PlanLength));
                SearchExpanderPanel.Children.Add(GenerateLabel("Plan Cost: ", _data.PlanCost));
                SearchExpanderPanel.Children.Add(GenerateLabel("Expanded States: ", _data.ExpandedStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Reopened States: ", _data.ReopenedStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Evaluated States: ", _data.EvaluatedStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Evaluations: ", _data.Evaluations));
                SearchExpanderPanel.Children.Add(GenerateLabel("Generated States: ", _data.GeneratedStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Dead End States: ", _data.DeadEndStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Expanded Until Last Jump States: ", _data.ExpandedUntilLastJumpStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Reopened Until Last Jump States: ", _data.ReopenedUntilLastJumpStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Evaluated Until Last Jump States: ", _data.EvaluatedUntilLastJumpStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Generated Until Last Jump States: ", _data.GeneratedUntilLastJumpStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Number Of Registered States: ", _data.NumberOfRegisteredStates));
                SearchExpanderPanel.Children.Add(GenerateLabel("Int Hash Set Load Factor: ", _data.IntHashSetLoadFactor));
                SearchExpanderPanel.Children.Add(GenerateLabel("Int Hash Set Resizes: ", _data.IntHashSetResizes));
            }
        }
    }
}