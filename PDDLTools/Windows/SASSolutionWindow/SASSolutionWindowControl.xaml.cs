using FastDownwardRunner.Models;
using PDDLParser.Models;
using PDDLTools.Helpers;
using PDDLTools.Options;
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

            var path = DirHelper.CombinePathAndFile(new FileInfo(OptionsAccessor.FDPPath).Directory.FullName, "sas_plan");
            if (!File.Exists(path))
            {
                TextPlan.Text = "'sas_plan' not found!";
            }
            else
            {
                List<PredicateExp> currentState = new List<PredicateExp>();
                foreach (var state in _pddlData.Problem.Init.Predicates)
                {
                    var name = state.Name.Split(' ')[0];
                    List<NameExp> args = new List<NameExp>();
                    foreach(var item in state.Name.Split(' '))
                        if (item != name && item != "")
                            args.Add(new NameExp(null, item));
                    currentState.Add(new PredicateExp(null, name, args));
                }
                TextPlan.Text = File.ReadAllText(path);
                foreach(var line in File.ReadLines(path))
                {
                    if (!line.Trim().StartsWith(";"))
                    {
                        bool isGoal = true;
                        bool isPartialGoal = false;
                        
                    }
                }
            }
        }
    }
}
