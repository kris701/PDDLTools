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
            FastDownwardPathTextbox.Text = OptionsAccessor.FDPPath;
            PythonPrefixTextbox.Text = OptionsAccessor.PythonPrefix;
            FastDownwardTimeoutNumericUpDown.Value = OptionsAccessor.FDFileExecutionTimeout;
            FastDownwardSearchOptionsTextbox.Text = OptionsAccessor.SearchOptions;
            OpenFDResultsOnSuccessCheckbox.Checked = OptionsAccessor.OpenResultReport;
            OpenSASResultOnSuccess.Checked = OptionsAccessor.OpenSASSolutionVisualiser;
            _isLoaded = true;
        }

        private void SelectFastDownwardPathButton_Click(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            FolderDialog.ShowDialog();
            if (FolderDialog.SelectedPath != "")
            {
                OptionsAccessor.FDPPath = FolderDialog.SelectedPath;
                FastDownwardPathTextbox.Text = OptionsAccessor.FDPPath;
            }
        }

        private void PythonPrefixTextbox_Leave(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsAccessor.PythonPrefix = PythonPrefixTextbox.Text;
            PythonPrefixTextbox.Text = OptionsAccessor.PythonPrefix;
        }

        private void FastDownwardTimeoutNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsAccessor.FDFileExecutionTimeout = (int)FastDownwardTimeoutNumericUpDown.Value;
            FastDownwardTimeoutNumericUpDown.Value = OptionsAccessor.FDFileExecutionTimeout;
        }

        private void FastDownwardSearchOptionsTextbox_Leave(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsAccessor.SearchOptions = FastDownwardSearchOptionsTextbox.Text;
            FastDownwardSearchOptionsTextbox.Text = OptionsAccessor.SearchOptions;
        }

        private void OpenSASResultOnSuccess_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsAccessor.OpenResultReport = OpenFDResultsOnSuccessCheckbox.Checked;
            OpenFDResultsOnSuccessCheckbox.Checked = OptionsAccessor.OpenResultReport;
        }

        private void OpenFDResultsOnSuccessCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (!_isLoaded)
                return;

            OptionsAccessor.OpenSASSolutionVisualiser = OpenSASResultOnSuccess.Checked;
            OpenSASResultOnSuccess.Checked = OptionsAccessor.OpenSASSolutionVisualiser;
        }
    }
}
