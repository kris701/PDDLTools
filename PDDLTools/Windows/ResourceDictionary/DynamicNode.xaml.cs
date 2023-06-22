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
        public Line NodeLine { get; internal set; }

        public int NodeID { get; }

        private DynamicNode _nextNode = null;
        private DynamicNode _prevNode = null;
        private bool _isSetup = false;
        private bool _isMouseDown = false;
        private Point _startLocation = new Point();

        public DynamicNode(int nodeID, string text, Canvas parent, Point? location = null)
        {
            InitializeComponent();

            ParentCanvas = parent;

            NodeID = nodeID;
            NodeTextLabel.Content = text;

            if (location != null)
            {
                Margin = new Thickness(
                    location.Value.X,
                    location.Value.Y,
                    0,
                    0);
            }

            NodeLine = new Line();
            NodeLine.Stroke = Brushes.WhiteSmoke;
            NodeLine.StrokeThickness = 3;
            NodeLine.X1 = Margin.Left + Width / 2;
            NodeLine.Y1 = Margin.Top + Height / 2;
            NodeLine.X2 = Margin.Left + Width / 2;
            NodeLine.Y2 = Margin.Top + Height / 2;
            NodeLine.Tag = NodeID;
            Canvas.SetZIndex(NodeLine, -2000);
            ParentCanvas.Children.Add(NodeLine);
        }

        public void Setup()
        {
            if (_isSetup)
                return;

            foreach (var child in ParentCanvas.Children)
            {
                if (child is DynamicNode otherNode)
                {
                    if (otherNode.NodeID == NodeID + 1)
                        _nextNode = otherNode;
                    if (otherNode.NodeID == NodeID - 1)
                        _prevNode = otherNode;
                }
            }
            UpdateLine();

            _isSetup = true;
        }

        public void UpdateLine()
        {
            if (_nextNode != null)
            {
                NodeLine.X1 = Margin.Left + Width / 2;
                NodeLine.Y1 = Margin.Top + Height / 2;

                NodeLine.X2 = _nextNode.Margin.Left + _nextNode.Width / 2;
                NodeLine.Y2 = _nextNode.Margin.Top + _nextNode.Height / 2;
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
                UpdateLine();
                if (_prevNode != null)
                    _prevNode.UpdateLine();
            }
        }
    }
}
