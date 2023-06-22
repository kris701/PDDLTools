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
        public int TargetNodeID { get; }

        private DynamicNode _targetNode = null;
        private bool _isSetup = false;

        public DynamicNode(int nodeID, string text, Canvas parent, int targetNodeID = -1, Point? location = null)
        {
            InitializeComponent();

            ParentCanvas = parent;

            NodeID = nodeID;
            TargetNodeID = targetNodeID;
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
                    if (otherNode.NodeID == TargetNodeID)
                    {
                        _targetNode = otherNode;
                        break;
                    }
                }
            }
            UpdateLine();

            _isSetup = true;
        }

        public void UpdateLine()
        {
            if (_targetNode != null)
            {
                NodeLine.X1 = Margin.Left + Width / 2;
                NodeLine.Y1 = Margin.Top + Height / 2;

                NodeLine.X2 = _targetNode.Margin.Left + _targetNode.Width / 2;
                NodeLine.Y2 = _targetNode.Margin.Top + _targetNode.Height / 2;
            }
        }
    }
}
