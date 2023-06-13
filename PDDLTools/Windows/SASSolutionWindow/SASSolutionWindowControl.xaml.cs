using PDDLParser.Models;
using PDDLTools.Helpers;
using PDDLTools.Models;
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
        public SASSolutionWindowControl()
        {
            InitializeComponent();
        }

        public void SetupResultData(FDResults data)
        {
            _data = data;
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
