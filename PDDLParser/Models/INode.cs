using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public interface INode
    {
        INode Parent { get; }
        int Start { get; set; }
        int End { get; set; }
        int Line { get; set; }

        HashSet<INode> FindNames(string name);
        HashSet<T> FindTypes<T>();
        bool Equals(object obj);
        int GetHashCode();
    }
}
