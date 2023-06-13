using FastDownwardRunner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDownwardRunner
{
    public interface IRunner
    {
        string FastDownwardFolder { get; }
        string PythonPrefix { get; }
        int Timeout { get; }

        List<LogItem> Log { get; }

        Task<FDResults> RunAsync(string domainPath, string problemPath, string searchArg);
    }
}
