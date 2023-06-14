using FastDownwardRunner.Models;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using PDDLTools.Helpers;
using PDDLTools.Options;
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
using static System.Windows.Forms.AxHost;

namespace PDDLTools.Windows.SASSolutionWindow
{
    public partial class SASSolutionWindowControl : UserControl
    {
        private FDResults _data;
        private PDDLDecl _pddlData;

        public SASSolutionWindowControl()
        {
            InitializeComponent();
        }

        public void SetupResultData(FDResults data, PDDLDecl pddlData)
        {
            _data = data;
            _pddlData = pddlData;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            while (_data == null)
                await Task.Delay(100);

            var path = Path.Combine(OptionsAccessor.FDPPath, "sas_plan");
            if (!File.Exists(path))
            {
                TextPlan.Text = "'sas_plan' not found!";
            }
            else
            {
                TextPlan.Text = File.ReadAllText(path);

                IPlanParser planParser = new PlanParser();
                var plan = planParser.ParsePlanFile(path);

                ISASSimulator simulator = new SASSimulator.SASSimulator(
                    _pddlData,
                    plan);

                var totalGoal = TotalGoalCount(_pddlData.Problem.Goal.GoalExp);

                for (int i = 0; i < plan.Count; i++)
                {
                    var goalCount = GoalStateCount(_pddlData.Problem.Goal.GoalExp, simulator.State);
                    bool isGoal = goalCount == totalGoal;
                    bool isPartialGoal = goalCount > 0;


                    simulator.Step();
                }
            }
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

        private int GoalStateCount(IExp exp, List<PredicateExp> state, bool inverse = false)
        {
            if (exp is AndExp and)
            {
                int count = 0;
                foreach (var child in and.Children)
                    count += GoalStateCount(child, state);
                return count;
            }
            else if (exp is NotExp not)
            {
                return GoalStateCount(not.Child, state, true);
            }
            else
            {
                if (exp is PredicateExp pred)
                {
                    if (inverse)
                        if (!state.Contains(pred))
                            return 1;
                    else
                        if (state.Contains(pred))
                            return 1;
                }
            }
            return 0;
        }
    }
}
