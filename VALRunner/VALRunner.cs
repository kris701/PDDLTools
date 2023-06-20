using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValRunner;

namespace VALRunner
{
    public class VALRunner : IRunner
    {
        public int Timeout { get; }

        public VALRunner(int timeout)
        {
            Timeout = timeout;
        }

        public Task<bool> IsPlanValid(string domainPath, string problemPath, string planPath)
        {
            throw new NotImplementedException();
        }
    }
}
