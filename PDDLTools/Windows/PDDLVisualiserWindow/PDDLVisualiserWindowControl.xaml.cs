﻿using LocationSpreader;
using PDDLParser;
using PDDLParser.Helpers;
using PDDLParser.Models;
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
        private string _selectedDomainFile = "";
        public string SelectedDomainFile { 
            get => _selectedDomainFile; 
            set {
                SelectedDomainFileLabel.Content = value;
                _selectedDomainFile = value;
                ConstructVisualiser();
            } 
        }

        public PDDLVisualiserWindowControl()
        {
            InitializeComponent();
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

        private void ConstructVisualiser()
        {
            if (PDDLHelper.IsFileDomain(SelectedDomainFile))
            {
                FirstStartLabel.Visibility = Visibility.Hidden;
                ErrorLabel.Visibility = Visibility.Hidden;
                MainGrid.Children.Clear();
                try
                {
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

                            var newNode = new DynamicNode(predDict[pred.Name], $"Predicate{Environment.NewLine} {pred.Name}", MainGrid, targetId, locs[index++]);
                            newNode.EllipseArea.Fill = Brushes.Red;
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

                            var newNode = new DynamicNode(actDict[act.Name], $"Action{Environment.NewLine} {act.Name}", MainGrid, targetId, locs[index++]);
                            newNode.EllipseArea.Fill = Brushes.Green;
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

                            var newNode = new DynamicNode(axiDict[currentAxiIndex], $"Axiom{Environment.NewLine} ID: {currentAxiIndex}", MainGrid, targetId, locs[index++]);
                            currentAxiIndex++;
                            newNode.EllipseArea.Fill = Brushes.Blue;
                            MainGrid.Children.Add(newNode);
                        }
                    }

                    foreach (var child in MainGrid.Children)
                        if (child is DynamicNode node)
                            node.Setup();

                    foreach (var child in MainGrid.Children)
                    {
                        if (child is DynamicNode node)
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
            ConstructVisualiser();
        }
    }
}
