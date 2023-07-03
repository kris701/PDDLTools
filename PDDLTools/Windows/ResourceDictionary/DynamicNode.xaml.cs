using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PDDLTools.Windows.ResourceDictionary
{
    public partial class DynamicNode : UserControl
    {
        public Canvas ParentCanvas { get; }
        public List<int> TargetIDs { get; }
        public List<ArrowLine> NodeLines { get; }
        public int NodeID { get; }
        public string Text { get; }

        private List<DynamicNode> _targetNodes = new List<DynamicNode>();
        private bool _isSetup = false;
        private bool _isMouseDown = false;
        private Point _startLocation = new Point();
        private Point _targetLocation = new Point();
        
        public DynamicNode(int nodeID, string text, Canvas parent, List<int> targetIDs, Point startLocation, Point targetLocation)
        {
            ParentCanvas = parent;

            NodeID = nodeID;
            Text = text;
            TargetIDs = targetIDs;

            InitializeComponent();

            _targetLocation = targetLocation;

            var toolTip = new ToolTip();
            toolTip.Content = text;
            ToolTip = toolTip;

            Margin = new Thickness(
                startLocation.X,
                startLocation.Y,
                0,
                0);

            NodeLines = new List<ArrowLine>();
            foreach(var target in targetIDs)
            {
                var newLine = new ArrowLine(
                    new Point(Margin.Left + Width / 2, Margin.Top + Height / 2),
                    new Point(Margin.Left + Width / 2, Margin.Top + Height / 2)
                    );
                Canvas.SetZIndex(newLine.Path, -2000);
                newLine.Path.Tag = nodeID;
                NodeLines.Add(newLine);
                ParentCanvas.Children.Add(newLine.Path);
            }
        }

        public bool MoveTowardsTarget()
        {
            if (Math.Abs(Margin.Left - _targetLocation.X) > 5 ||
                Math.Abs(Margin.Top - _targetLocation.Y) > 5)
            {
                Margin = new Thickness(
                    Margin.Left + (_targetLocation.X - Margin.Left) / 2,
                    Margin.Top + (_targetLocation.Y - Margin.Top) / 2,
                    0, 0);
                UpdateLines();
                return false;
            }
            UpdateLines();
            return true;
        }

        public void Setup()
        {
            if (_isSetup)
                return;

            foreach (var child in ParentCanvas.Children)
            {
                if (child is DynamicNode otherNode)
                {
                    if (TargetIDs.Contains(otherNode.NodeID))
                        _targetNodes.Add(otherNode);
                }
            }
            UpdateLines();

            _isSetup = true;
        }

        public void UpdateLines()
        {
            for(int i = 0; i < _targetNodes.Count; i++)
            {
                NodeLines[i].UpdateLine(
                    new Point(
                        Margin.Left + Width / 2, 
                        Margin.Top + Height / 2),
                    new Point(
                        _targetNodes[i].Margin.Left + _targetNodes[i].Width / 2,
                        _targetNodes[i].Margin.Top + _targetNodes[i].Height / 2));
            }
        }


        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isMouseDown = true;
                _startLocation = e.GetPosition(ParentCanvas);
                Mouse.Capture(this);
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isMouseDown = false;
                Mouse.Capture(null);
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var location = e.GetPosition(ParentCanvas);
                Margin = new Thickness(
                        _startLocation.X - (_startLocation.X - location.X) - (Width / 2),
                        _startLocation.Y - (_startLocation.Y - location.Y) - (Height / 2),
                        0,
                        0);

                UpdateLines();

                foreach (var child in ParentCanvas.Children)
                    if (child is DynamicNode node)
                        if (node.TargetIDs.Contains(NodeID))
                            node.UpdateLines();
            }
        }
    }
}
