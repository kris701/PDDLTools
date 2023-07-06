using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public interface IASTParser<T>
    {
        T Parse(string text);
        string TokenizeSpecials(string text);
    }
}
