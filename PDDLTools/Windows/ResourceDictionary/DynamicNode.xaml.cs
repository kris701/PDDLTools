using System;
using System.Collections.Generic;
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

namespace PDDLTools.Windows.ResourceDictionary
{
    public partial class DynamicNode : UserControl
    {
        public Canvas ParentCanvas { get; }
        public List<int> TargetIDs { get; }
        public List<Line> NodeLines { get; }
        public int NodeID { get; }

        private List<DynamicNode> _targetNodes = new List<DynamicNode>();
        private bool _isSetup = false;
        private bool _isMouseDown = false;
        private Point _startLocation = new Point();
        
        public DynamicNode(int nodeID, string text, Canvas parent, List<int> targetIDs, Point? location = null)
        {
            InitializeComponent();

            ParentCanvas = parent;

            NodeID = nodeID;
            NodeTextLabel.Content = text;
            TargetIDs = targetIDs;

            if (location != null)
            {
                Margin = new Thickness(
                    location.Value.X,
                    location.Value.Y,
                    0,
                    0);
            }

            NodeLines = new List<Line>();
            foreach(var target in targetIDs)
            {
                var newLine = new Line();
                newLine.Stroke = Brushes.WhiteSmoke;
                newLine.StrokeThickness = 3;
                newLine.X1 = Margin.Left + Width / 2;
                newLine.Y1 = Margin.Top + Height / 2;
                newLine.X2 = Margin.Left + Width / 2;
                newLine.Y2 = Margin.Top + Height / 2;
                newLine.Tag = NodeID;
                Canvas.SetZIndex(newLine, -2000);
                NodeLines.Add(newLine);
                ParentCanvas.Children.Add(newLine);
            }
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
                NodeLines[i].X1 = Margin.Left + Width / 2;
                NodeLines[i].Y1 = Margin.Top + Height / 2;

                NodeLines[i].X2 = _targetNodes[i].Margin.Left + _targetNodes[i].Width / 2;
                NodeLines[i].Y2 = _targetNodes[i].Margin.Top + _targetNodes[i].Height / 2;
            }
        }


        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _isMouseDown = true;
                _startLocation = e.GetPosition(ParentCanvas);
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                _isMouseDown = false;
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
