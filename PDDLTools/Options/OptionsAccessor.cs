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
        public static OptionsPageCustom Instance = null;

        public static string FDPPath
        {
            get { return Instance.FDPath; }
            set { Instance.FDPath = value; }
        }

        public static string PythonPrefix
        {
            get { return Instance.PythonPrefix; }
            set { Instance.PythonPrefix = value; }
        }

        public static int FDFileExecutionTimeout
        {
            get { return Instance.FDFileExecutionTimeout; }
            set { Instance.FDFileExecutionTimeout = value; }
        }

        public static bool IsFirstStart
        {
            get { return Instance.IsFirstStart; }
            set { Instance.IsFirstStart = value; }
        }

        public static string SearchOptions
        {
            get { return Instance.SearchOptions; }
            set { Instance.SearchOptions = value; }
        }

        public static bool OpenResultReport
        {
            get { return Instance.OpenResultReport; }
            set { Instance.OpenResultReport = value; }
        }

        public static bool OpenSASSolutionVisualiser
        {
            get { return Instance.OpenSASSolutionVisualiser; }
            set { Instance.OpenSASSolutionVisualiser = value; }
        }
    }
}
