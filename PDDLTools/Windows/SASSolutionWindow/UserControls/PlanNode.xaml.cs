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

namespace PDDLTools.Windows.SASSolutionWindow.UserControls
{
    public partial class PlanNode : UserControl
    {
        public PlanNode(int id, string tooltip, bool isGoal, bool isPartialGoal)
        {
            InitializeComponent();

            NodeID.Content = id;
            EllipseArea.ToolTip = tooltip;
            if (isGoal)
                EllipseArea.Fill = Brushes.Green;
            else if (isPartialGoal)
                EllipseArea.Fill = Brushes.Orange;
        }
    }
}
