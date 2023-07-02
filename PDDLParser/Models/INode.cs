using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public interface INode
    {
        int Start { get; set; }
        int End { get; set; }
        int Line { get; set; }

        HashSet<INode> FindName(string name);
        bool Equals(object obj);
        int GetHashCode();
    }
}
