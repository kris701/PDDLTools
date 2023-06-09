using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;

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
                Instance.FDPath = value;
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
                Instance.FDFileExecutionTimeout = value;
                Instance.SaveSettingsToStorage();
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
    }
}
