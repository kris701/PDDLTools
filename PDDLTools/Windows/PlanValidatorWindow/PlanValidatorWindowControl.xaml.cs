using CMDRunners.VAL;
using PDDLParser.Helpers;
using PDDLTools.Commands;
using PDDLTools.Options;
using PDDLTools.Projects;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace PDDLTools.Windows.PlanValidatorWindow
{
    public partial class PlanValidatorWindowControl : UserControl
    {

        public PlanValidatorWindowControl()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await ReloadCheckerAsync();
        }

        public async Task ReloadCheckerAsync()
        {
            if (File.Exists(Path.Combine(OptionsManager.Instance.VALPath, "Validate.exe")))
            {
                MainGrid.IsEnabled = true;
                MainGrid.Opacity = 1;
                IsVALFoundLabel.Visibility = Visibility.Hidden;

                SelectedDomainFileLabel.Text = SelectDomainCommand.SelectedDomainPath;
                SelectedProblemFileLabel.Text = SelectProblemCommand.SelectedProblemPath;

                await DoCheckVALAsync();
            }
        }

        private async Task DoCheckVALAsync()
        {
            string domainFile = SelectedDomainFileLabel.Text as string;
            string problemFile = SelectedProblemFileLabel.Text as string;
            string planFile = SelectedPlanFileLabel.Text as string;

            bool autoCheck = true;
            if (!File.Exists(domainFile))
                autoCheck = false;
            if (!File.Exists(problemFile))
                autoCheck = false;
            if (!File.Exists(planFile))
                autoCheck = false;

            if (autoCheck)
            {
                VALRunner runner = new VALRunner(OptionsManager.Instance.VALPath, 10);
                var isValid = await runner.IsPlanValid(domainFile, problemFile, planFile);

                if (isValid)
                {
                    IsValidLabel.Content = "Plan is Valid!";
                    IsValidLabel.Foreground = Brushes.DarkGreen;
                }
                else
                {
                    IsValidLabel.Content = "Plan is not valid!";
                    IsValidLabel.Foreground = Brushes.Red;
                }
            }
        }

        private async void SelectDomainFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            fileSelector.Filter = "pddl files (*.pddl)|*.pddl";
            if (File.Exists(SelectedDomainFileLabel.Text as string))
                fileSelector.InitialDirectory = new FileInfo(SelectedDomainFileLabel.Text as string).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                if (PDDLHelper.IsFileDomain(fileSelector.FileName))
                {
                    SelectedDomainFileLabel.Text = fileSelector.FileName;
                    await DoCheckVALAsync();
                }
                else
                    MessageBox.Show("Selected file is not a domain file!");
            }
        }

        private async void SelectProblemFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            fileSelector.Filter = "pddl files (*.pddl)|*.pddl";
            if (File.Exists(SelectedProblemFileLabel.Text as string))
                fileSelector.InitialDirectory = new FileInfo(SelectedProblemFileLabel.Text as string).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                if (PDDLHelper.IsFileProblem(fileSelector.FileName))
                {
                    SelectedProblemFileLabel.Text = fileSelector.FileName;
                    await DoCheckVALAsync();
                }
                else
                    MessageBox.Show("Selected file is not a problem file!");
            }
        }

        private async void SelectPlanFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            if (File.Exists(SelectedPlanFileLabel.Text as string))
                fileSelector.InitialDirectory = new FileInfo(SelectedPlanFileLabel.Text as string).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                SelectedPlanFileLabel.Text = fileSelector.FileName;
                await DoCheckVALAsync();
            }
        }
    }
}
