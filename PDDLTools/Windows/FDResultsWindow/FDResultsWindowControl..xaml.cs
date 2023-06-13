using PDDLTools.Models;
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
        private bool _isLoaded = false;
        public FDResultsWindowControl()
        {
            InitializeComponent();
        }

        public async Task SetupResultDataAsync(FDResults data)
        {
            while(!_isLoaded)
                await Task.Delay(100);

            ResultsTitleLabel.Content = $"Fast Downward Run Results: {DateTime.Now}";

            GeneralExpanderPanel.Children.Clear();
            TranslatorExpanderPanel.Children.Clear();
            SearchExpanderPanel.Children.Clear();

            GeneralExpanderPanel.Children.Add(GenerateLabel("Solution found: " ,data.WasSolutionFound));
            GeneralExpanderPanel.Children.Add(GenerateLabel("Search Time (s): " ,data.SearchTime));
            GeneralExpanderPanel.Children.Add(GenerateLabel("Total Time (s): ", data.TotalTime));
            GeneralExpanderPanel.Children.Add(GenerateLabel("Fast Downward Exit Code: " , Enum.GetName(typeof(FDExitCode), data.ExitCode)));

            TranslatorExpanderPanel.Children.Add(GenerateLabel("Variables: ", data.TranslatorVariables));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Derived Variables: ", data.TranslatorDerivedVariables));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Facts: ", data.TranslatorFacts));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Goal Facts: ", data.TranslatorGoalFacts));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("MutexGroups: ", data.TranslatorMutexGroups));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Total Mutex Groups Size: ", data.TranslatorTotalMutexGroupsSize));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Operators: ", data.TranslatorOperators));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Axioms: ", data.TranslatorAxioms));
            TranslatorExpanderPanel.Children.Add(GenerateLabel("Task Size: ", data.TranslatorTaskSize));

            SearchExpanderPanel.Children.Add(GenerateLabel("Plan Length: ", data.PlanLength));
            SearchExpanderPanel.Children.Add(GenerateLabel("Plan Cost: ", data.PlanCost));
            SearchExpanderPanel.Children.Add(GenerateLabel("Expanded States: ", data.ExpandedStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Reopened States: ", data.ReopenedStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Evaluated States: ", data.EvaluatedStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Evaluations: ", data.Evaluations));
            SearchExpanderPanel.Children.Add(GenerateLabel("Generated States: ", data.GeneratedStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Dead End States: ", data.DeadEndStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Expanded Until Last Jump States: ", data.ExpandedUntilLastJumpStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Reopened Until Last Jump States: ", data.ReopenedUntilLastJumpStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Evaluated Until Last Jump States: ", data.EvaluatedUntilLastJumpStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Generated Until Last Jump States: ", data.GeneratedUntilLastJumpStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Number Of Registered States: ", data.NumberOfRegisteredStates));
            SearchExpanderPanel.Children.Add(GenerateLabel("Int Hash Set Load Factor: ", data.IntHashSetLoadFactor));
            SearchExpanderPanel.Children.Add(GenerateLabel("Int Hash Set Resizes: ", data.IntHashSetResizes));

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }
    }
}