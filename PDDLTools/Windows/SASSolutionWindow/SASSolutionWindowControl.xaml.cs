using FastDownwardRunner.Models;
using LocationSpreader;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using PDDLTools.Helpers;
using PDDLTools.Options;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.AxHost;

namespace PDDLTools.Windows.SASSolutionWindow
{
    public partial class SASSolutionWindowControl : UserControl
    {
        private FDResults _data;
        private PDDLDecl _pddlData;
        private bool _redraw = false;

        public SASSolutionWindowControl()
        {
            InitializeComponent();
        }

        public void SetupResultData(FDResults data, PDDLDecl pddlData)
        {
            _data = data;
            _pddlData = pddlData;
            _redraw = true;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            while (_data == null)
                await Task.Delay(100);

            if (_redraw)
            {
                var path = System.IO.Path.Combine(OptionsAccessor.FDPPath, "sas_plan");
                if (!File.Exists(path))
                {
                    TextPlan.Text = "'sas_plan' not found!";
                }
                else
                {
                    SetTextPlanData(File.ReadAllLines(path));
                    VisualPlan.Children.Clear();

                    IPlanParser planParser = new PlanParser();
                    var plan = planParser.ParsePlanFile(path);

                    if (plan.Count > 50)
                    {
                        PlanTooLargeLable.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        PlanTooLargeLable.Visibility = Visibility.Hidden;
                        ISASSimulator simulator = new SASSimulator.SASSimulator(
                            _pddlData,
                            plan);

                        var lines = GenerateBaseLines(plan.Count);

                        ILocationSpreader spreader = new RandomPathCorrectingSpreader();
                        var locs = spreader.GenerateSuitableLocations((int)VisualPlan.ActualWidth, (int)VisualPlan.ActualHeight, plan.Count + 1, 50);

                        var prevNode = AddNewNode(0, "init", simulator.State, _pddlData.Problem.Goal.GoalExpCount, locs[0]);

                        for (int i = 0; i < plan.Count; i++)
                        {
                            simulator.Step();
                            var newNode = AddNewNode(i + 1, $"Step {i + 1}: {plan[i]}", simulator.State, _pddlData.Problem.Goal.GoalExpCount, locs[i + 1]);

                            MakeLineBetweenNodes(prevNode, newNode, lines[i]);

                            prevNode = newNode;
                        }
                    }
                }
                _redraw = false;
            }
        }

        private List<Line> GenerateBaseLines(int count)
        {
            var lines = new List<Line>();
            for (int i = 0; i < count; i++)
            {
                var newLine = new Line();
                newLine.Stroke = Brushes.Black;
                newLine.StrokeThickness = 3;
                lines.Add(newLine);
                VisualPlan.Children.Add(newLine);
            }
            return lines;
        }

        private void MakeLineBetweenNodes(PlanNode a, PlanNode b, Line newLine)
        {
            newLine.X1 = a.Margin.Left + a.Width / 2;
            newLine.Y1 = a.Margin.Top + a.Height / 2;

            newLine.X2 = b.Margin.Left + b.Width / 2;
            newLine.Y2 = b.Margin.Top + b.Height / 2;
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
                    TextPlan.Text += Environment.NewLine;
                    TextPlan.Text += $"Plan Cost: {planCost}{Environment.NewLine}";
                }
                else
                {
                    TextPlan.Text += $"[{i + 1}] {lines[i]}{Environment.NewLine}";
                }
            }
        }

        private PlanNode AddNewNode(int id, string text, List<PredicateExp> state, int totalGoal, Point loc)
        {
            var goalCount = GetGoalCountInState(_pddlData.Problem.Goal.GoalExp, state);
            bool isGoal = goalCount == totalGoal;
            bool isPartialGoal = goalCount > 0;
            var newNode = new PlanNode(id, text, isGoal, isPartialGoal);
            newNode.Margin = new Thickness(loc.X, loc.Y, 0, 0);
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
    }
}
