using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.ErrorList.PDDLParser.Exceptions
{
    public class ParseException : Exception
    {
        public ErrorTask Error { get; private set; }
        public ParseException(string message, TaskErrorCategory category, int line, TaskPriority priority) : base(message)
        {
            ErrorTask newError = new ErrorTask();
            newError.ErrorCategory = category;
            newError.Text = message;
            newError.Line = line;
            newError.Document = "";
            newError.Priority = priority;
            Error = newError;
        }
    }
}
