using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.TestAdapter.Models
{
    public class PDDLTest
    {
        public string Domain { get; set; }
        public List<string> Problems { get; set; }
        public List<string> Tasks { get; set; }
    }
}
