using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PDDLTools.Options
{
    public static class OptionsAccessor
    {
        public static OptionPageGrid Instance = null;

        public static string FDPPath
        {
            get
            {
                return Instance.FDPath;
            }
            set
            {
                if (Directory.Exists(value) && Directory.Exists(Path.Combine(value, "fast-downward.py")))
                {
                    Instance.FDPath = value;
                    Instance.SaveSettingsToStorage();
                }
                else
                    MessageBox.Show("Error, path to fast downward!");
            }
        }

        public static string PythonPrefix
        {
            get
            {
                return Instance.PythonPrefix;
            }
            set
            {
                Instance.PythonPrefix = value;
                Instance.SaveSettingsToStorage();
            }
        }

        public static int FDFileExecutionTimeout
        {
            get
            {
                return Instance.FDFileExecutionTimeout;
            }
            set 
            {
                if (value > 0)
                {
                    Instance.FDFileExecutionTimeout = value;
                    Instance.SaveSettingsToStorage();
                }
                else
                    MessageBox.Show("Timeout must be larger than 0!");
            }
        }

        public static bool IsFirstStart
        {
            get
            {
                return Instance.IsFirstStart;
            }
            set
            {
                Instance.IsFirstStart = value;
                Instance.SaveSettingsToStorage();
            }
        }

        public static string SearchOptions
        {
            get
            {
                return Instance.SearchOptions;
            }
            set
            {
                Instance.SearchOptions = value;
                Instance.SaveSettingsToStorage();
            }
        }

        public static bool OpenResultReport
        {
            get
            {
                return Instance.OpenResultReport;
            }
            set
            {
                Instance.OpenResultReport = value;
                Instance.SaveSettingsToStorage();
            }
        }

        public static bool OpenSASSolutionVisualiser
        {
            get
            {
                return Instance.OpenSASSolutionVisualiser;
            }
            set
            {
                Instance.OpenSASSolutionVisualiser = value;
                Instance.SaveSettingsToStorage();
            }
        }
    }
}
