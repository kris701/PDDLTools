using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public interface INode
    {
        int Character { get; set; }
        int Line { get; set; }
    }
}
