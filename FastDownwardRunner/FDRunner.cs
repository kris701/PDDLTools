using FastDownwardRunner.Helpers;
using FastDownwardRunner.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDownwardRunner
{
    public class FDRunner : IRunner
    {
        public string FastDownwardFolder { get; }
        public string PythonPrefix { get; }
        public int Timeout { get; }
        public List<LogItem> Log { get; internal set; }

        public FDRunner(string fastDownwardFolder, string pythonPrefix, int timeout)
        {
            FastDownwardFolder = fastDownwardFolder;
            PythonPrefix = pythonPrefix;
            Timeout = timeout;
            Log = new List<LogItem>();
        }

        public async Task<FDResults> RunAsync(string domainPath, string problemPath, string searchArg)
        {
            Log.Clear();

            var process = new PowershellProcess();
            process.ErrorDataRecieved += RecieveErrorData;
            process.OutputDataRecieved += RecieveOutputData;
            process.StopOnError = true;
            await process.StartProcessAsync($"& {PythonPrefix} '{Path.Combine(FastDownwardFolder, "fast-downward.py")}' --plan-file '{Path.Combine(FastDownwardFolder, "sas_plan")}' '{domainPath}' '{problemPath}' --search '{searchArg}'");

            var timeoutSpan = TimeSpan.FromSeconds(Timeout);
            var res = await process.WaitForExitAsync(timeoutSpan);

            return new FDResults(Log, res);
        }

        private void RecieveErrorData(object sender, DataReceivedEventArgs e)
        {
            Log.Add(new LogItem(e.Data, LogItem.ItemType.Error));
        }

        private void RecieveOutputData(object sender, DataReceivedEventArgs e)
        {
            Log.Add(new LogItem(e.Data, LogItem.ItemType.Log));
        }
    }
}
