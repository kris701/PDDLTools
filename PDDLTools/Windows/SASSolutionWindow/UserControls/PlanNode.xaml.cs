using PDDLParser.Models;
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
using static System.Windows.Forms.AxHost;

namespace PDDLTools.Windows.SASSolutionWindow.UserControls
{
    public partial class PlanNode : UserControl
    {
        public List<PredicateExp> State { get; }
        public PlanNode(int id, string actionStep, List<PredicateExp> state, bool isGoal, bool isPartialGoal)
        {
            InitializeComponent();

            State = state;
            Tag = id;
            NodeID.Content = id;
            var toolTip = new ToolTip();
            toolTip.BorderThickness = new Thickness(0);
            toolTip.Background = Brushes.Transparent;
            toolTip.Content = new StateTooltip(actionStep, isPartialGoal, isGoal, state);
            this.ToolTip = toolTip;
            if (isGoal)
                EllipseArea.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#227023");
            else if (isPartialGoal)
                EllipseArea.Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#877b12");
        }
    }
}
