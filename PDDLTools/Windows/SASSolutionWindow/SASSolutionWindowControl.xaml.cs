using FastDownwardRunner.Models;
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

                        var lines = new List<Line>();
                        for(int i = 0; i < plan.Count; i++)
                        {
                            var newLine = new Line();
                            newLine.Stroke = Brushes.Black;
                            newLine.StrokeThickness = 3;
                            lines.Add(newLine);
                            VisualPlan.Children.Add(newLine);
                        }
                        var locs = GenerateSuitableLocations((int)VisualPlan.ActualWidth, (int)VisualPlan.ActualHeight, plan.Count + 1, 50);

                        var totalGoal = TotalGoalCount(_pddlData.Problem.Goal.GoalExp);
                        var prevNode = AddNewNode(0, "init", simulator.State, totalGoal, locs[0]);

                        for (int i = 0; i < plan.Count; i++)
                        {
                            simulator.Step();
                            var newNode = AddNewNode(i + 1, $"Step {i + 1}: {plan[i]}", simulator.State, totalGoal, locs[i + 1]);

                            MakeLineBetweenNodes(prevNode, newNode, lines[i]);

                            prevNode = newNode;
                        }
                    }
                }
                _redraw = false;
            }
        }

        private List<Point> GenerateSuitableLocations(int width, int height, int count, int radius)
        {
            Random rnd = new Random();
            List<Point> points = new List<Point>();
            points.Add(new Point(
                        rnd.Next(radius, width - radius),
                        rnd.Next(radius, height - radius)
                        ));
            for (int i = 0; i < count - 1; i++)
            {
                var newPoint = new Point();
                double dist = double.MaxValue;
                newPoint = new Point(
                        rnd.Next(radius, width - radius),
                        rnd.Next(radius, height - radius)
                        );
                while (GetClosestPointDistance(points, newPoint) < radius * 3)
                    newPoint = new Point(
                        rnd.Next(radius, width - radius),
                        rnd.Next(radius, height - radius)
                        );

                points.Add(newPoint);
            }

            bool changed = true;
            int tries = 0;
            int maxTries = 1000;
            while (changed)
            {
                changed = false;
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var dist = Distance(points[i], points[i + 1]);
                    for (int j = i + 2; j < points.Count; j++)
                    {
                        var newDist = Distance(points[i], points[j]);
                        if (newDist < dist)
                        {
                            var point1 = points[i + 1];
                            var point2 = points[j];
                            points[j] = point1;
                            points[i + 1] = point2;
                            changed = true;
                            break;
                        }
                    }
                }
                if (!changed)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var a1 = points[i];
                        var a2 = points[i + 1];

                        for (int j = i + 2; j < points.Count - 1; j++)
                        {
                            var b1 = points[j];
                            var b2 = points[j + 1];

                            if (Intersect(a1, a2, b1, b2))
                            {
                                var rid1 = rnd.Next(0, points.Count);
                                while (rid1 == i && rid1 != i + 1)
                                    rid1 = rnd.Next(0, points.Count);

                                var rp1 = points[rid1];
                                points[rid1] = a1;
                                points[i] = rp1;

                                changed = true;
                                i = points.Count;
                                break;
                            }
                        }
                    }
                }
                tries++;
                if (tries > maxTries)
                    break;
            }

            return points;
        }

        private double GetClosestPointDistance(List<Point> points, Point target)
        {
            double dist = double.MaxValue;
            foreach (var point in points)
            {
                var newDist = Distance(point, target);
                if (newDist < dist)
                    dist = newDist;
            }
            return dist;
        }

        private double Distance(Point a, Point b) => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        private bool ccw(Point A, Point B, Point C) => (C.Y - A.Y) * (B.X - A.X) > (B.Y - A.Y) * (C.X - A.X);
        private bool Intersect(Point A, Point B, Point C, Point D) => ccw(A, C, D) != ccw(B, C, D) && ccw(A, B, C) != ccw(A, B, D);


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
            var goalCount = _pddlData.Problem.Goal.GoalExpCount;
            bool isGoal = goalCount == totalGoal;
            bool isPartialGoal = goalCount > 0;
            var newNode = new PlanNode(id, text, isGoal, isPartialGoal);
            newNode.Margin = new Thickness(loc.X, loc.Y, 0, 0);
            VisualPlan.Children.Add(newNode);
            return newNode;
        }

        private int TotalGoalCount(IExp exp)
        {
            if (exp is AndExp and)
            {
                int count = 0;
                foreach (var child in and.Children)
                    count += TotalGoalCount(child);
                return count;
            }
            else if (exp is NotExp not)
            {
                return TotalGoalCount(not.Child);
            }
            return 1;
        }
    }
}
