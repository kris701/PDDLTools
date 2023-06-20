using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValRunner
{
    public interface IRunner
    {
        int Timeout { get; }
        Task<bool> IsPlanValid(string domainPath, string problemPath, string planPath);
    }
}
