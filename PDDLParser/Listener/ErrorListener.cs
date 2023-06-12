using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Listener
{
    public class ErrorListener : IErrorListener
    {
        public ParseErrorType ThrowIfTypeAbove { get; set; }

        private List<ParseError> _errors = new List<ParseError>();
        public void AddError(ParseError err)
        {
            _errors.Add(err);
            if (_errors.Any(x => x.Type > ThrowIfTypeAbove))
                throw new ParseException(_errors);
        }
    }
}
