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
            FastDownwardSearchOptionsTextbox.Text = OptionsManager.Instance.SearchOptions;
            OpenFDResultsOnSuccessCheckbox.Checked = OptionsManager.Instance.OpenResultReport;
            OpenSASResultOnSuccess.Checked = OptionsManager.Instance.OpenSASSolutionVisualiser;
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

            OptionsManager.Instance.SearchOptions = FastDownwardSearchOptionsTextbox.Text;
            FastDownwardSearchOptionsTextbox.Text = OptionsManager.Instance.SearchOptions;
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
    }
}
