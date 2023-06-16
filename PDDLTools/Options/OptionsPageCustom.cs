using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDDLTools.Options
{
    [Guid("1D32BA32-4249-4291-9539-1CB4C9FE9C88")]
    public class OptionsPageCustom : DialogPage
    {
        private string _FDPath = "";
        public string FDPath { 
            get { return _FDPath; }
            set {
                if (Directory.Exists(value) && File.Exists(Path.Combine(value, "fast-downward.py")))
                {
                    _FDPath = value;
                    SaveSettingsToStorage();
                }
                else
                    MessageBox.Show("Error, path to fast downward!");
            } 
        }

        private string _PythonPrefix = "python";
        public string PythonPrefix
        {
            get { return _PythonPrefix; }
            set { _PythonPrefix = value; }
        }

        private int _FDFileExecutionTimeout = 10;
        public int FDFileExecutionTimeout
        {
            get { return _FDFileExecutionTimeout; }
            set {
                if (value > 0)
                {
                    _FDFileExecutionTimeout = value;
                    SaveSettingsToStorage();
                }
                else
                    MessageBox.Show("Timeout must be larger than 0!");
            }
        }

        private bool _IsFirstStart = true;
        public bool IsFirstStart
        {
            get { return _IsFirstStart; }
            set { _IsFirstStart = value; }
        }

        private string _SearchOptions = "astar(lmcut());lazy_greedy([ff(), cea()], [ff(), cea()])";
        public string SearchOptions
        {
            get { return _SearchOptions; }
            set { _SearchOptions = value; }
        }

        private bool _OpenResultReport = true;
        public bool OpenResultReport
        {
            get { return _OpenResultReport; }
            set { _OpenResultReport = value; }
        }

        private bool _OpenSASSolutionVisualiser = true;
        public bool OpenSASSolutionVisualiser
        {
            get { return _OpenSASSolutionVisualiser; }
            set { _OpenSASSolutionVisualiser = value; }
        }

        protected override IWin32Window Window
        {
            get
            {
                OptionsPageCustomControl page = new OptionsPageCustomControl();
                return page;
            }
        }
    }
}
