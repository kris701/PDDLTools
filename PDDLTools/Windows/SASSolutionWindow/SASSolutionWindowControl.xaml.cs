using LocationSpreader;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using PDDLTools.Helpers;
using PDDLTools.Options;
using PDDLTools.Windows.ResourceDictionary;
using PDDLTools.Windows.SASSolutionWindow.UserControls;
using SASSimulator;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;

namespace PDDLTools.Windows.SASSolutionWindow
{
    public partial class SASSolutionWindowControl : UserControl
    {
        private PDDLDecl _pddlData;
        private string _planFile;
        private bool _reDraw = false;
        private bool _isLoaded = false;

        public SASSolutionWindowControl()
        {
            InitializeComponent();

            SelectSpreaderCombobox.Items.Clear();
            foreach (var spreader in LocationSpreaderBuilder.Spreaders)
                SelectSpreaderCombobox.Items.Add(spreader);
            SelectSpreaderCombobox.SelectedIndex = 0;
        }

        public void SetupResultData(PDDLDecl pddlData, string planFile)
        {
            _pddlData = pddlData;
            _planFile = planFile;
            _reDraw = true;
            RedrawCanvas();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            while (_pddlData == null)
                await Task.Delay(100);

            _isLoaded = true;

            RedrawCanvas();
        }

        private void RedrawCanvas()
        {
            if (_reDraw && _isLoaded)
            {
                if (!File.Exists(_planFile))
                {
                    TextPlan.Text = $"The plan file '{_planFile}' not found!";
                }
                else
                {
                    SetTextPlanData(File.ReadAllLines(_planFile));
                    VisualPlan.Children.Clear();

                    IPlanParser planParser = new PlanParser();
                    var plan = planParser.ParsePlanFile(_planFile);

                    if (plan.Count > 50)
                    {
                        PlanTooLargeLable.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PlanTooLargeLable.Visibility = Visibility.Hidden;

                        SimulationStepSlider.Maximum = plan.Count;
                        SimulationStepSlider.Value = SimulationStepSlider.Maximum;

                        ISASSimulator simulator = new SASSimulator.SASSimulator(
                            _pddlData,
                            plan);


                        ILocationSpreader spreader = LocationSpreaderBuilder.GetSpreader(SelectSpreaderCombobox.SelectedItem as string);
                        var locs = spreader.GenerateSuitableLocations((int)VisualPlan.ActualWidth, (int)VisualPlan.ActualHeight, plan.Count + 1, 50);

                        AddNewNode(0, "Start Step", simulator.State, _pddlData.Problem.Goal.PredicateCount, locs[0]);

                        for (int i = 0; i < plan.Count; i++)
                        {
                            simulator.Step();
                            AddNewNode(i + 1, $"{i + 1}: {plan[i]}", simulator.State, _pddlData.Problem.Goal.PredicateCount, locs[i + 1]);
                        }

                        foreach (var child in VisualPlan.Children)
                            if (child is DynamicNode node)
                                node.Setup();
                    }

                    _reDraw = false;
                }
            }
        }

        private void SetTextPlanData(string[] lines)
        {
            TextPlan.Text = "";
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(";"))
                {
                    var fromIndex = lines[i].IndexOf("=") + 1;
                    var toIndex = lines[i].IndexOf("(");
                    var planCost = lines[i].Substring(fromIndex, toIndex - fromIndex).Trim();
                    PlanLengthLabel.Content = planCost;
                }
                else
                {
                    TextPlan.Text += $"[{i + 1}] {lines[i]}{Environment.NewLine}";
                }
            }
        }

        private DynamicNode AddNewNode(int id, string text, List<PredicateExp> state, int totalGoal, Point loc)
        {
            var goalCount = GetGoalCountInState(_pddlData.Problem.Goal.GoalExp, state);
            bool isGoal = goalCount == totalGoal;
            bool isPartialGoal = goalCount > 0;
            List<PredicateExp> cloneState = new List<PredicateExp>();
            foreach(var pred in state)
                cloneState.Add(pred.Clone() as PredicateExp);
            var newNode = new DynamicNode(id, $"{id}", VisualPlan, new List<int>() { id + 1 }, loc);

            var toolTip = new ToolTip();
            toolTip.BorderThickness = new Thickness(0);
            toolTip.Background = Brushes.Transparent;
            toolTip.Content = new StateTooltip(text, isPartialGoal, isGoal, state);
            newNode.ToolTip = toolTip;
            if (isGoal)
                newNode.EllipseArea.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#227023");
            else if (isPartialGoal)
                newNode.EllipseArea.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#877b12");

            VisualPlan.Children.Add(newNode);
            return newNode;
        }

        private int GetGoalCountInState(IExp exp, List<PredicateExp> state)
        {
            if (exp is AndExp and)
            {
                int count = 0;
                foreach (var child in and.Children)
                    count += GetGoalCountInState(child, state);
                return count;
            }
            else if (exp is NotExp not)
            {
                return GetGoalCountInState(not.Child, state);
            }
            else if (exp is OrExp or)
            {
                return GetGoalCountInState(or.Option1, state) + GetGoalCountInState(or.Option2, state);
            }
            else
            {
                if (exp is PredicateExp pred)
                {
                    if (state.Contains(pred))
                        return 1;
                }
            }
            return 0;
        }

        private bool _isFading = false;
        private async void SimulationStepSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            while (_isFading)
                await Task.Delay(100);

            int newIndex = (int)SimulationStepSlider.Value;
            SimulationStepLabel.Content = $"{newIndex}";

            List<UIElement> fadeInElements = new List<UIElement>();
            List<UIElement> fadeOutElements = new List<UIElement>();

            foreach (var child in VisualPlan.Children)
            {
                if (child is System.Windows.Shapes.Path path)
                {
                    if (path.Tag is int id)
                    {
                        if (id + 1 > newIndex)
                        {
                            if (path.Visibility == Visibility.Visible)
                                fadeOutElements.Add(path);
                        }
                        else
                        {
                            if (path.Visibility == Visibility.Hidden)
                                fadeInElements.Add(path);
                        }
                    }
                }
                else if (child is DynamicNode node)
                {
                    if (node.NodeID > newIndex)
                    {
                        if (node.Visibility == Visibility.Visible)
                            fadeOutElements.Add(node);
                    }
                    else
                    {
                        if (node.Visibility == Visibility.Hidden)
                            fadeInElements.Add(node);
                    }
                }
            }

            _isFading = true;
            await FadeOutFastAsync(fadeOutElements);
            await FadeInFastAsync(fadeInElements);
            _isFading = false;
        }

        private async Task FadeInFastAsync(List<UIElement> elements)
        {
            foreach (var element in elements)
                element.Opacity = 0;
            foreach (var element in elements)
                element.Visibility = Visibility.Visible;

            for (double i = 0; i < 1; i += 0.1)
            {
                foreach (var element in elements)
                    element.Opacity = i;
                await Task.Delay(10);
            }
            foreach (var element in elements)
                element.Opacity = 1;
        }

        private async Task FadeOutFastAsync(List<UIElement> elements)
        {
            foreach (var element in elements)
                element.Opacity = 1;
            foreach (var element in elements)
                element.Visibility = Visibility.Visible;

            for (double i = 1; i > 0; i -= 0.1)
            {
                foreach (var element in elements)
                    element.Opacity = i;
                await Task.Delay(10);
            }
            foreach (var element in elements)
                element.Opacity = 0;

            foreach (var element in elements)
                element.Visibility = Visibility.Hidden;
        }

        private void RerollButton_Click(object sender, RoutedEventArgs e)
        {
            _reDraw = true;
            RedrawCanvas();
        }

        private void SelectSpreaderCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded)
            {
                _reDraw = true;
                RedrawCanvas();
            }
        }
    }
}
