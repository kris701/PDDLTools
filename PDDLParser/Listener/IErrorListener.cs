using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Listener
{
    public interface IErrorListener
    {
        ParseErrorType ThrowIfTypeAbove { get; set; }
        void AddError(ParseError err);
    }
}
