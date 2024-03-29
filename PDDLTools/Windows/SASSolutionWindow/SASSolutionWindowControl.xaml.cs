﻿using LocationSpreader;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using PDDLTools.Helpers;
using PDDLTools.Options;
using PDDLTools.Windows.ResourceDictionary;
using PDDLTools.Windows.SASSolutionWindow.UserControls;
using SASSimulator;
using SASSimulator.Models;
using SASSimulator.Parsers;
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

        public async void SetupResultData(PDDLDecl pddlData, string planFile)
        {
            _pddlData = pddlData;
            _planFile = planFile;
            _reDraw = true;
            await RedrawCanvasAsync();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            while (_pddlData == null)
                await Task.Delay(100);

            _isLoaded = true;

            await RedrawCanvasAsync();
        }

        private async Task RedrawCanvasAsync()
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

                        if (_pddlData.Problem == null || _pddlData.Domain == null)
                        {
                            CannotParseLabel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            CannotParseLabel.Visibility = Visibility.Hidden;
                            ISASSimulator simulator = new SASSimulator.SASSimulator(
                                _pddlData,
                                plan);

                            ILocationSpreader spreader = LocationSpreaderBuilder.GetSpreader(SelectSpreaderCombobox.SelectedItem as string);
                            var locs = spreader.GenerateSuitableLocations((int)VisualPlan.ActualWidth, (int)VisualPlan.ActualHeight, plan.Count + 1, 50);

                            var centerPoint = new Point((int)VisualPlan.ActualWidth / 2, (int)VisualPlan.ActualHeight / 2);
                            List<DynamicNode> nodes = new List<DynamicNode>();

                            int index = 0;
                            nodes.Add(AddNewNode(0, "Start Step", simulator.State, _pddlData.Problem.Goal.PredicateCount, centerPoint, locs[index++], plan.Count));

                            for (int i = 0; i < plan.Count; i++)
                            {
                                simulator.Step();
                                nodes.Add(AddNewNode(i + 1, $"{i + 1}: {plan[i]}", simulator.State, _pddlData.Problem.Goal.PredicateCount, centerPoint, locs[index++], plan.Count));
                            }

                            foreach (var node in nodes)
                                node.Setup();

                            bool isAllThere = false;
                            while (!isAllThere)
                            {
                                isAllThere = true;
                                foreach (var node in nodes)
                                    if (!node.MoveTowardsTarget())
                                        isAllThere = false;
                                await Task.Delay(50);
                            }
                        }
                    }

                    _reDraw = false;
                }
            }
        }

        private void SetTextPlanData(string[] lines)
        {
            TextPlan.Text = "";
            int length = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(";"))
                {
                    var fromIndex = lines[i].IndexOf("=") + 1;
                    var toIndex = lines[i].IndexOf("(");
                    var planCost = lines[i].Substring(fromIndex, toIndex - fromIndex).Trim();
                    PlanCostLabel.Content = planCost;
                    PlanLengthLabel.Content = length;
                }
                else
                {
                    TextPlan.Text += $"[{i + 1}] {lines[i]}{Environment.NewLine}";
                    length++;
                }
            }
        }

        private DynamicNode AddNewNode(int id, string text, HashSet<Predicate> state, int totalGoal, Point start, Point target, int totalSteps)
        {
            var goalCount = GetGoalCountInState(_pddlData.Problem.Goal.GoalExp as INode, state);
            bool isGoal = goalCount == totalGoal;
            bool isPartialGoal = goalCount > 0;
            List<Predicate> cloneState = new List<Predicate>();
            foreach(var pred in state)
                cloneState.Add(pred.Clone() as Predicate);
            List<int> targetIds = new List<int>();
            if (id != totalSteps)
                targetIds.Add(id + 1);
            var newNode = new DynamicNode(id, $"{id}", VisualPlan, targetIds, start, target);

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

        private int GetGoalCountInState(INode exp, HashSet<Predicate> state)
        {
            if (exp is PredicateExp pred)
            {
                if (state.Contains(new Predicate(pred)))
                    return 1;
            } else if (exp is IWalkable walk)
            {
                int count = 0;
                foreach (var item in walk)
                    count += GetGoalCountInState(item, state);
                return count;
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

        private async void RerollButton_Click(object sender, RoutedEventArgs e)
        {
            _reDraw = true;
            await RedrawCanvasAsync();
        }

        private async void SelectSpreaderCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isLoaded)
            {
                _reDraw = true;
                await RedrawCanvasAsync();
            }
        }
    }
}
