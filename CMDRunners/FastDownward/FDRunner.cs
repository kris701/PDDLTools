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

        public async Task<FDResults> RunAsync(string domainPath, string problemPath, string engineArg)
        {
            SetupRunner();
            StringBuilder sb = new StringBuilder("& ");
            sb.Append($"{PythonPrefix} ");
            sb.Append($"'{Path.Combine(ExecutablePath, ExecutableName)}' ");
            sb.Append($"--plan-file '{Path.Combine(ExecutablePath, "sas_plan")}' ");
            sb.Append($"--sas-file '{Path.Combine(ExecutablePath, "output.sas")}' ");

            engineArg = engineArg.Replace("\"", "'");
            if (engineArg.ToLower().Contains("--alias"))
                sb.Append($"{engineArg} ");

            sb.Append($"'{domainPath}' ");
            sb.Append($"'{problemPath}' ");

            if (engineArg.ToLower().Contains("--search"))
                sb.Append($"{engineArg}");

            await Process.StartProcessAsync(sb.ToString());
            var res = await Process.WaitForExitAsync();

            return new FDResults(Log, res);
        }

        public async Task<List<string>> GetAliasesAsync()
        {
            List<string> aliases = new List<string>();
            SetupRunner();
            StringBuilder sb = new StringBuilder("& ");
            sb.Append($"{PythonPrefix} ");
            sb.Append($"'{Path.Combine(ExecutablePath, ExecutableName)}' ");
            sb.Append($"--show-aliases ");

            await Process.StartProcessAsync(sb.ToString());
            var res = await Process.WaitForExitAsync();

            foreach(var item in Log)
                if (item != null && item.Type != LogItem.ItemType.Error)
                    aliases.Add(item.Content.Trim());

            return aliases;
        }
    }
}
