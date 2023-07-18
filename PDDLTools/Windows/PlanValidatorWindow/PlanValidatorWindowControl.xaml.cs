using CMDRunners.VAL;
using PDDLParser.Helpers;
using PDDLTools.Commands;
using PDDLTools.Options;
using PDDLTools.Projects;
using PDDLTools.Projects.PDDLProject;
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
        public delegate void DoVALCheckHandler();
        public event DoVALCheckHandler DoVALCheck;

        private string _selectedDomainFile = "";
        public string SelectedDomainFile { 
            get => _selectedDomainFile; 
            set {
                if (!PDDLHelper.IsFileDomain(value))
                    return;
                _selectedDomainFile = value;
                SelectedDomainFileLabel.Text = new FileInfo(value).Name;
                if (DoVALCheck != null) DoVALCheck.Invoke();
            } 
        }
        private string _selectedProblemFile = "";
        public string SelectedProblemFile { 
            get => _selectedProblemFile; 
            set {
                if (!PDDLHelper.IsFileProblem(value))
                    return;
                _selectedProblemFile = value;
                SelectedProblemFileLabel.Text = new FileInfo(value).Name;
                if (DoVALCheck != null) DoVALCheck.Invoke();
            } 
        }
        private string _selectedPlanFile = "";
        public string SelectedPlanFile { 
            get => _selectedPlanFile; 
            set {
                if (!PDDLHelper.IsFilePlan(value))
                    return;
                _selectedPlanFile = value;
                SelectedPlanFileLabel.Text = new FileInfo(value).Name;
                if (DoVALCheck != null) DoVALCheck.Invoke();
            } 
        }

        public PlanValidatorWindowControl()
        {
            InitializeComponent();
            DoVALCheck += DoCheckVAL_Trigger;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(Path.Combine(OptionsManager.Instance.VALPath, "Validate.exe")))
            {
                MainGrid.IsEnabled = true;
                MainGrid.Opacity = 1;
                IsVALFoundLabel.Visibility = Visibility.Hidden;

                var proj = await PDDLProjectManager.GetCurrentProjectAsync();
                if (proj != null)
                {
                    if (PDDLHelper.IsFileDomain(await proj.GetSelectedDomainAsync()))
                        SelectedDomainFile = await proj.GetSelectedDomainAsync();
                    if (PDDLHelper.IsFileProblem(await proj.GetSelectedProblemAsync()))
                        SelectedProblemFile = await proj.GetSelectedProblemAsync();
                }
            }
        }

        private async void DoCheckVAL_Trigger()
        {
            IsValidLabel.Content = "Running VAL...";
            IsValidLabel.Foreground = Brushes.Orange;

            VALRunner runner = new VALRunner(OptionsManager.Instance.VALPath, 10);
            var isValid = await runner.IsPlanValid(SelectedDomainFile, SelectedProblemFile, SelectedPlanFile);

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

        private void SelectDomainFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            fileSelector.Filter = "pddl files (*.pddl)|*.pddl";
            if (File.Exists(SelectedDomainFile))
                fileSelector.InitialDirectory = new FileInfo(SelectedDomainFile).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                if (PDDLHelper.IsFileDomain(fileSelector.FileName))
                    SelectedDomainFile = fileSelector.FileName;
                else
                    MessageBox.Show("Selected file is not a domain file!");
            }
        }

        private void SelectProblemFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            fileSelector.Filter = "pddl files (*.pddl)|*.pddl";
            if (File.Exists(SelectedProblemFile))
                fileSelector.InitialDirectory = new FileInfo(SelectedProblemFile).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                if (PDDLHelper.IsFileProblem(fileSelector.FileName))
                    SelectedProblemFile = fileSelector.FileName;
                else
                    MessageBox.Show("Selected file is not a problem file!");
            }
        }

        private void SelectPlanFileButton_Click(object sender, RoutedEventArgs e)
        {
            var fileSelector = new System.Windows.Forms.OpenFileDialog();
            fileSelector.Filter = "pddl plan files (*.pddlplan)|*.pddlplan";
            if (File.Exists(SelectedPlanFile))
                fileSelector.InitialDirectory = new FileInfo(SelectedPlanFile).Directory.FullName;
            fileSelector.ShowDialog();
            if (fileSelector.FileName != "")
            {
                if (PDDLHelper.IsFilePlan(fileSelector.FileName))
                    SelectedPlanFile = fileSelector.FileName;
                else
                    MessageBox.Show("Selected file is not a plan file!");
            }
        }

        private void RecheckButton_Click(object sender, RoutedEventArgs e)
        {
            DoVALCheck.Invoke();
        }
    }
}
