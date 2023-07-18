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
        // Our logger used to display messages
        protected TestLogger TestLog;
        // The adapter version
        private readonly string adapterVersion;

        protected bool UseVsKeepEngineRunning { get; private set; }
        public bool ShadowCopy { get; private set; }
        public bool CollectSourceInformation { get; private set; }

        public int Verbosity { get; private set; }


        protected bool RegistryFailure { get; set; }
        protected string ErrorMsg
        {
            get; set;
        }

        #region Constructor

        protected PDDLTestAdapter()
        {
            Verbosity = 0;
            RegistryFailure = false;
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
            CollectSourceInformation = GetInnerTextAsBool(runConfiguration, "CollectSourceInformation", true);
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

        #endregion

        private const string Name = "PDDLTools VS Adapter";

        protected void Info(string method, string function)
        {
            var msg = string.Format("{0} {1} {2} is {3}", Name, adapterVersion, method, function);
            TestLog.SendInformationalMessage(msg);
        }

        protected void Debug(string method, string function)
        {
#if DEBUG
            var msg = string.Format("{0} {1} {2} is {3}", Name, adapterVersion, method, function);
            TestLog.SendDebugMessage(msg);
#endif
        }

        #endregion
    }
}
