using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDDLTools.Options
{
    public partial class OptionsPageCustomControl : UserControl
    {
        public OptionsPageCustomControl()
        {
            InitializeComponent();
        }

        private bool _isLoaded = false;

        private void OptionsPageCustomControl_Load(object sender, EventArgs e)
        {
            FastDownwardPathTextbox.Text = OptionsManager.Instance.FDPath;
            PythonPrefixTextbox.Text = OptionsManager.Instance.PythonPrefix;
            FastDownwardTimeoutNumericUpDown.Value = OptionsManager.Instance.FDFileExecutionTimeout;
            FastDownwardSearchOptionsTextbox.Text = OptionsManager.Instance.EngineOptions;
            OpenFDResultsOnSuccessCheckbox.Checked = OptionsManager.Instance.OpenResultReport;
            OpenSASResultOnSuccess.Checked = OptionsManager.Instance.OpenSASSolutionVisualiser;
            EnableEditorMarginCheckbox.Checked = OptionsManager.Instance.EnableEditorMargin;
            EnableSyntaxHighlightingCheckbox.Checked = OptionsManager.Instance.EnableSyntaxHighlighting;
            EnableAutoCompletementofStatementsCheckbox.Checked = OptionsManager.Instance.EnableAutoCompleteOfStatements;
            EnableErrorCheckingCheckbox.Checked = OptionsManager.Instance.EnableErrorCheckingOnSave;
            EnableBraceMatchingCheckbox.Checked = OptionsManager.Instance.EnableBraceMatching;
            EnableWordHighlightingCheckbox.Checked = OptionsManager.Instance.EnableHighlightingOfWords;
            IntermediateOutputPathTextbox.Text = OptionsManager.Instance.IntermediateOutputPath;
            OutputPlanPath.Text = OptionsManager.Instance.OutputPlanPath;
            _isLoaded = true;
        }

        private void SelectFastDownwardPathButton_Click(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            FolderDialog.ShowDialog();
            if (FolderDialog.SelectedPath != "")
            {
                OptionsManager.Instance.FDPath = FolderDialog.SelectedPath;
                FastDownwardPathTextbox.Text = OptionsManager.Instance.FDPath;
            }
        }

        private void VALPathButton_Click(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            FolderDialog.ShowDialog();
            if (FolderDialog.SelectedPath != "")
            {
                OptionsManager.Instance.VALPath = FolderDialog.SelectedPath;
                VALPathTextbox.Text = OptionsManager.Instance.VALPath;
            }
        }

        private void PythonPrefixTextbox_Leave(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.PythonPrefix = PythonPrefixTextbox.Text;
            PythonPrefixTextbox.Text = OptionsManager.Instance.PythonPrefix;
        }

        private void FastDownwardTimeoutNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.FDFileExecutionTimeout = (int)FastDownwardTimeoutNumericUpDown.Value;
            FastDownwardTimeoutNumericUpDown.Value = OptionsManager.Instance.FDFileExecutionTimeout;
        }

        private void FastDownwardSearchOptionsTextbox_Leave(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EngineOptions = FastDownwardSearchOptionsTextbox.Text;
            FastDownwardSearchOptionsTextbox.Text = OptionsManager.Instance.EngineOptions;
        }

        private void OpenSASResultOnSuccess_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.OpenResultReport = OpenFDResultsOnSuccessCheckbox.Checked;
            OpenFDResultsOnSuccessCheckbox.Checked = OptionsManager.Instance.OpenResultReport;
        }

        private void OpenFDResultsOnSuccessCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.OpenSASSolutionVisualiser = OpenSASResultOnSuccess.Checked;
            OpenSASResultOnSuccess.Checked = OptionsManager.Instance.OpenSASSolutionVisualiser;
        }

        private void EnableEditorMarginCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EnableEditorMargin = EnableEditorMarginCheckbox.Checked;
            EnableEditorMarginCheckbox.Checked = OptionsManager.Instance.EnableEditorMargin;
        }

        private void EnableSyntaxHighlightingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EnableSyntaxHighlighting = EnableSyntaxHighlightingCheckbox.Checked;
            EnableSyntaxHighlightingCheckbox.Checked = OptionsManager.Instance.EnableSyntaxHighlighting;
        }

        private void EnableAutoCompletementofStatementsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EnableAutoCompleteOfStatements = EnableAutoCompletementofStatementsCheckbox.Checked;
            EnableAutoCompletementofStatementsCheckbox.Checked = OptionsManager.Instance.EnableAutoCompleteOfStatements;
        }

        private void EnableErrorCheckingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EnableErrorCheckingOnSave = EnableErrorCheckingCheckbox.Checked;
            EnableErrorCheckingCheckbox.Checked = OptionsManager.Instance.EnableErrorCheckingOnSave;
        }

        private void EnableBraceMatchingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EnableBraceMatching = EnableBraceMatchingCheckbox.Checked;
            EnableBraceMatchingCheckbox.Checked = OptionsManager.Instance.EnableBraceMatching;
        }

        private void EnableWordHighlightingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.EnableHighlightingOfWords = EnableWordHighlightingCheckbox.Checked;
            EnableWordHighlightingCheckbox.Checked = OptionsManager.Instance.EnableHighlightingOfWords;
        }

        private void IntermediateOutputPathTextbox_Leave(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.IntermediateOutputPath = IntermediateOutputPathTextbox.Text;
            IntermediateOutputPathTextbox.Text = OptionsManager.Instance.IntermediateOutputPath;
        }

        private void OutputPlanPathTextbox_Leave(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsManager.Instance.OutputPlanPath = OutputPlanPathTextbox.Text;
            OutputPlanPathTextbox.Text = OptionsManager.Instance.OutputPlanPath;
        }
    }
}
