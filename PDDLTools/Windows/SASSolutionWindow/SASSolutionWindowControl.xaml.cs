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
        public SASSolutionWindowControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var path = DirHelper.CombinePathAndFile(new FileInfo(OptionsAccessor.FDPPath).Directory.FullName, "sas_plan");
            if (!File.Exists(path))
            {
                TextPlan.Text = "'sas_plan' not found!";
            }
            else
            {
                TextPlan.Text = File.ReadAllText(path);
            }
        }
    }
}
