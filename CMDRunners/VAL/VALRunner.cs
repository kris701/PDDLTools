using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDRunners.VAL
{
    public class VALRunner : BaseRunner
    {
        public VALRunner(string executablePath, int timeout) : base(executablePath, "Validate.exe", timeout)
        {
        }

        public async Task<bool> IsPlanValid(string domainFile, string problemFile, string planFile)
        {
            SetupRunner();
            await Process.StartProcessAsync($"& '{Path.Combine(ExecutablePath, ExecutableName)}' '{domainFile}' '{problemFile}' '{planFile}'");

            var res = await Process.WaitForExitAsync();

            if (res != Helpers.ProcessCompleteReson.RanToCompletion)
                return false;

            foreach(var item in Log)
                if (item.Content == "Plan Valid" && item.Type != Models.LogItem.ItemType.Error)
                    return true;
            return false;
        }
    }
}
