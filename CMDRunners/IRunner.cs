using CMDRunners.Helpers;
using CMDRunners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDRunners
{
    public interface IRunner
    {
        string ExecutablePath { get; }
        string ExecutableName { get; }
        int Timeout { get; }
        List<LogItem> Log { get; }

        PowershellProcess Process { get; }
        bool VerifyExecutable();
    }
}
