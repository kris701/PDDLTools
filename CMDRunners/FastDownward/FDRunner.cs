using CMDRunners.Helpers;
using CMDRunners.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDRunners.FastDownward
{
    public class FDRunner : BaseRunner
    {
        public string PythonPrefix { get; }

        public FDRunner(string executablePath, string pythonPrefix, int timeout) : base(executablePath, "fast-downward.py", timeout)
        {
            PythonPrefix = pythonPrefix;
        }

        public async Task<FDResults> RunAsync(string domainPath, string problemPath, string searchArg)
        {
            SetupRunner();
            await Process.StartProcessAsync($"& {PythonPrefix} '{Path.Combine(ExecutablePath, ExecutableName)}' --plan-file '{Path.Combine(ExecutablePath, "sas_plan")}' --sas-file '{Path.Combine(ExecutablePath, "output.sas")}' '{domainPath}' '{problemPath}' --search '{searchArg}'");

            var res = await Process.WaitForExitAsync();

            return new FDResults(Log, res);
        }
    }
}
