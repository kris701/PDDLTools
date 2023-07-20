using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PDDLTools.TestAdapter
{
    public abstract class PDDLTestAdapter
    {
        protected TestLogger TestLog;

        public int Verbosity { get; private set; }

        public string FastDownwardPath { get; set; }
        public string FastDownwardEngineArgs { get; set; }
        public string PythonPrefix { get; set; }
        public int FastDownwardTimeout { get; set; }
        public bool RunParallel { get; set; }


        #region Constructor

        protected PDDLTestAdapter()
        {
            Verbosity = 0;
            TestLog = new TestLogger(Verbosity);
        }

        #endregion

        #region Protected Helper Methods

        protected void Initialize(IDiscoveryContext context)
        {
            var settingsXml = context?.RunSettings?.SettingsXml;
            if (string.IsNullOrEmpty(settingsXml))
                settingsXml = "<RunSettings />";
            var doc = new XmlDocument();
            doc.LoadXml(settingsXml);
            var runConfiguration = doc.SelectSingleNode("RunSettings/RunConfiguration");
            FastDownwardPath = GetInnerTextAsString(runConfiguration, "FDPath", "fast-downward.py");
            FastDownwardEngineArgs = GetInnerTextAsString(runConfiguration, "FDEngineArgs", "\"astar(lmcut())\"");
            PythonPrefix = GetInnerTextAsString(runConfiguration, "PythonPrefix", "python");
            FastDownwardTimeout = GetInnerTextAsInt(runConfiguration, "FDTimeout", 10);
            RunParallel = GetInnerTextAsBool(runConfiguration, "RunParallel", false);
        }

        #region Helper Methods

        private string GetInnerText(XmlNode startNode, string xpath, params string[] validValues)
        {
            if (startNode != null)
            {
                var targetNode = startNode.SelectSingleNode(xpath);
                if (targetNode != null)
                {
                    string val = targetNode.InnerText;

                    if (validValues != null && validValues.Length > 0)
                    {
                        foreach (string valid in validValues)
                            if (string.Compare(valid, val, StringComparison.OrdinalIgnoreCase) == 0)
                                return valid;

                        throw new ArgumentException(string.Format(
                            "Invalid value {0} passed for element {1}.", val, xpath));
                    }

                    return val;
                }
            }

            return null;
        }

        private bool GetInnerTextAsBool(XmlNode startNode, string xpath, bool defaultValue)
        {
            string temp = GetInnerText(startNode, xpath);

            if (string.IsNullOrEmpty(temp))
                return defaultValue;

            return bool.Parse(temp);
        }

        private int GetInnerTextAsInt(XmlNode startNode, string xpath, int defaultValue)
        {
            string temp = GetInnerText(startNode, xpath);

            if (string.IsNullOrEmpty(temp))
                return defaultValue;

            return int.Parse(temp);
        }

        private string GetInnerTextAsString(XmlNode startNode, string xpath, string defaultValue)
        {
            string temp = GetInnerText(startNode, xpath);

            if (string.IsNullOrEmpty(temp))
                return defaultValue;

            return temp;
        }

        #endregion

        private const string Name = "PDDLTools VS Adapter";

        protected void Info(string method, string function)
        {
            var msg = string.Format("{0} {1} is {3}", Name, method, function);
            TestLog.SendInformationalMessage(msg);
        }

        protected void Debug(string method, string function)
        {
#if DEBUG
            var msg = string.Format("{0} {1} is {3}", Name,method, function);
            TestLog.SendDebugMessage(msg);
#endif
        }

        #endregion
    }
}
