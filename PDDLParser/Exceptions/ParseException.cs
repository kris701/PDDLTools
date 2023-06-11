using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Exceptions
{
    public enum ParserErrorLevel { None, Low, Medium, High }
    public enum ParseErrorCategory { None, Message, Warning, Error }
    public class ParseException : Exception
    {
        public ParserErrorLevel Level { get; internal set; }
        public ParseErrorCategory Category { get; internal set; }
        public int Line { get; internal set; }

        public ParseException(string message, ParserErrorLevel level, ParseErrorCategory category, int line) : base(message)
        {
            Level = level;
            Category = category;
            Line = line;
        }
    }
}
