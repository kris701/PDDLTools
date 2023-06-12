using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Exceptions
{
    public class ParseException : Exception
    {
        public List<ParseError> Errors { get; internal set; }
        public ParseException(List<ParseError> errors)
        {
            Errors = errors;
        }
    }
}
