using PDDLParser.Models;
using SASSimulator.Models;
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
    public partial class StateTooltip : UserControl
    {
        public HashSet<Predicate> State { get; }
        public StateTooltip(string actionStep, bool isPartial, bool isGoal, HashSet<Predicate> state)
        {
            InitializeComponent();
            SimulationStepLabel.Text = actionStep;
            State = state;

            IsPartialCheckbox.IsChecked = isPartial;
            if (isPartial)
                IsPartialCheckbox.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#877b12");
            IsGoalCheckbox.IsChecked = isGoal;
            if (isGoal)
                IsGoalCheckbox.Foreground = (SolidColorBrush)new BrushConverter().ConvertFrom("#227023");

            foreach (var step in State)
                StateText.Text += $"{step}{Environment.NewLine}";
        }
    }
}
