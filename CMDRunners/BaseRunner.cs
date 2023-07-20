using CMDRunners.Helpers;
using CMDRunners.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDRunners
{
    public abstract class BaseRunner : IRunner
    {
        public string ExecutablePath { get; }
        public string ExecutableName { get; }
        public int Timeout { get; }
        public List<LogItem> Log { get; }
        public PowershellProcess Process { get; internal set; }

        public BaseRunner(string executablePath, string executableName, int timeout)
        {
            ExecutablePath = executablePath;
            ExecutableName = executableName;
            Timeout = timeout;
            Log = new List<LogItem>();
        }

        internal void SetupRunner()
        {
            Log.Clear();

            Process = new PowershellProcess();
            Process.OutputTimeout = TimeSpan.FromSeconds(Timeout);
            Process.ErrorDataRecieved += RecieveErrorData;
            Process.OutputDataRecieved += RecieveOutputData;
            Process.StopOnError = true;
        }

        private void RecieveErrorData(object sender, DataReceivedEventArgs e)
        {
            Log.Add(new LogItem(e.Data, LogItem.ItemType.Error));
        }

        private void RecieveOutputData(object sender, DataReceivedEventArgs e)
        {
            Log.Add(new LogItem(e.Data, LogItem.ItemType.Log));
        }

        public abstract bool VerifyExecutable();
    }
}
