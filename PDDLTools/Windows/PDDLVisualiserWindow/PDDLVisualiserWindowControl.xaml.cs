using LocationSpreader;
using PDDLParser;
using PDDLParser.Helpers;
using PDDLParser.Models;
using PDDLTools.Commands;
using PDDLTools.Projects;
using PDDLTools.Windows.ResourceDictionary;
using PDDLTools.Windows.SASSolutionWindow.UserControls;
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
using static System.Net.Mime.MediaTypeNames;

namespace PDDLTools.Windows.PDDLVisualiserWindow
{
    public partial class PDDLVisualiserWindowControl : UserControl
    {
        public delegate void UpdateVisualiserHandler();
        public event UpdateVisualiserHandler UpdateVisualiser;

        private string _selectedDomainFile = "";
        public string SelectedDomainFile { 
            get => _selectedDomainFile; 
            set {
                SelectedDomainFileLabel.Content = value;
                _selectedDomainFile = value;
                UpdateVisualiser.Invoke();
            } 
        }

        public PDDLVisualiserWindowControl()
        {
            InitializeComponent();
            UpdateVisualiser += ConstructVisualiser;
        }

        private void SelectDomainFile_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            fileSelector.Filter = "pddl files (*.pddl)|*.pddl";
            if (File.Exists(SelectedDomainFile))
                fileSelector.InitialDirectory = new FileInfo(SelectedDomainFile).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                if (PDDLHelper.IsFileDomain(fileSelector.FileName))
                    SelectedDomainFile = fileSelector.FileName;
                else
                    MessageBox.Show("Selected file is not a domain file!");
            }
        }

        private async void ConstructVisualiser()
        {
            if (PDDLHelper.IsFileDomain(SelectedDomainFile))
            {
                FirstStartLabel.Visibility = Visibility.Hidden;
                ErrorLabel.Visibility = Visibility.Hidden;
                MainGrid.Children.Clear();
                try
                {
                    var nodes = new List<DynamicNode>();
                    var centerPoint = new Point((int)MainGrid.ActualWidth / 2, (int)MainGrid.ActualHeight / 2);

                    IPDDLParser parser = new PDDLParser.PDDLParser(false, false);
                    var decl = parser.Parse(SelectedDomainFile);
                    int locCounter = 0;

                    var predDict = new Dictionary<string, int>();
                    if (decl.Domain.Predicates != null)
                        foreach (var pred in decl.Domain.Predicates.Predicates)
                            predDict.Add(pred.Name, locCounter++);

                    var actDict = new Dictionary<string, int>();
                    if (decl.Domain.Actions != null)
                        foreach (var act in decl.Domain.Actions)
                            actDict.Add(act.Name, locCounter++);

                    var axiDict = new Dictionary<int, int>();
                    int axiIndex = 0;
                    if (decl.Domain.Axioms != null)
                        foreach (var axi in decl.Domain.Axioms)
                            axiDict.Add(axiIndex++, locCounter++);

                    ILocationSpreader spreader = new RandomSpreader();
                    var locs = spreader.GenerateSuitableLocations((int)MainGrid.ActualWidth, (int)MainGrid.ActualHeight, locCounter, 50);

                    int index = 0;
                    if (decl.Domain.Predicates != null)
                    {
                        foreach (var pred in decl.Domain.Predicates.Predicates)
                        {
                            List<int> targetId = new List<int>();
                            foreach (var act in decl.Domain.Actions)
                                if (IsPredicateUsed(act.Preconditions, pred.Name))
                                    targetId.Add(actDict[act.Name]);

                            var newNode = new DynamicNode(predDict[pred.Name], $"Predicate{Environment.NewLine} {pred.Name}", MainGrid, targetId, centerPoint);
                            newNode.EllipseArea.Fill = Brushes.Red;
                            nodes.Add(newNode);
                            MainGrid.Children.Add(newNode);
                        }
                    }

                    if (decl.Domain.Actions != null)
                    {
                        foreach (var act in decl.Domain.Actions)
                        {
                            List<int> targetId = new List<int>();
                            foreach (var pred in decl.Domain.Predicates.Predicates)
                                if (IsPredicateUsed(act.Effects, pred.Name))
                                    targetId.Add(predDict[pred.Name]);

                            var newNode = new DynamicNode(actDict[act.Name], $"Action{Environment.NewLine} {act.Name}", MainGrid, targetId, centerPoint);
                            newNode.EllipseArea.Fill = Brushes.Green;
                            nodes.Add(newNode);
                            MainGrid.Children.Add(newNode);
                        }
                    }

                    if (decl.Domain.Axioms != null)
                    {
                        int currentAxiIndex = 0;
                        foreach (var axi in decl.Domain.Axioms)
                        {
                            List<int> targetId = new List<int>();
                            foreach (var pred in decl.Domain.Predicates.Predicates)
                                if (IsPredicateUsed(axi.Implies, pred.Name))
                                    targetId.Add(predDict[pred.Name]);

                            var newNode = new DynamicNode(axiDict[currentAxiIndex], $"Axiom{Environment.NewLine} ID: {currentAxiIndex}", MainGrid, targetId, centerPoint);
                            currentAxiIndex++;
                            newNode.EllipseArea.Fill = Brushes.Blue;
                            nodes.Add(newNode);
                            MainGrid.Children.Add(newNode);
                        }
                    }

                    foreach (var node in nodes)
                        node.Setup();

                    foreach (var node in nodes)
                    {
                        bool doesAnyTargetThis = false;
                        foreach (var otherChild in MainGrid.Children)
                        {
                            if (otherChild is DynamicNode node2)
                            {
                                if (node.NodeID != node2.NodeID)
                                {
                                    if (node2.TargetIDs.Contains(node.NodeID))
                                    {
                                        doesAnyTargetThis = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!doesAnyTargetThis)
                        {
                            node.EllipseArea.Fill = Brushes.Gray;
                            foreach (var line in node.NodeLines)
                            {
                                line.Path.Stroke = Brushes.DarkGray;
                                line.Path.Fill = Brushes.DarkGray;
                                line.Path.StrokeDashArray = new DoubleCollection() { 2 };
                            }
                        }
                    }

                    bool isAllThere = false;
                    while (!isAllThere)
                    {
                        isAllThere = true;
                        for (int i = 0; i < nodes.Count; i++)
                        {
                            if (Math.Abs(nodes[i].Margin.Left - locs[i].X) > 5 ||
                                Math.Abs(nodes[i].Margin.Top - locs[i].Y) > 5)
                            {
                                nodes[i].Margin = new Thickness(
                                    nodes[i].Margin.Left + (locs[i].X - nodes[i].Margin.Left) / 2,
                                    nodes[i].Margin.Top + (locs[i].Y - nodes[i].Margin.Top) / 2,
                                    0, 0);
                                isAllThere = false;
                            }
                            nodes[i].UpdateLines();
                        }
                        await Task.Delay(50);
                    }
                }
                catch
                {
                    ErrorLabel.Visibility = Visibility.Visible;
                }
            }
        }

        private static bool IsPredicateUsed(IExp exp, string predicate)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (IsPredicateUsed(child, predicate))
                        return true;
            }
            else if (exp is OrExp or)
            {
                if (IsPredicateUsed(or.Option1, predicate))
                    return true;
                if (IsPredicateUsed(or.Option2, predicate))
                    return true;
            }
            else if (exp is NotExp not)
            {
                return IsPredicateUsed(not.Child, predicate);
            }
            else if (exp is PredicateExp pred)
            {
                if (pred.Name == predicate)
                    return true;
            }
            return false;
        }

        private void RerollButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateVisualiser.Invoke();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var proj = await PDDLProjectManager.GetCurrentProjectAsync();
            if (proj != null)
                if (PDDLHelper.IsFileDomain(await proj.GetSelectedDomainAsync()))
                    SelectedDomainFile = await proj.GetSelectedDomainAsync();
        }
    }
}
