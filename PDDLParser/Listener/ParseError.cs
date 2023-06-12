using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Listener
{
    public enum ParserErrorLevel { None, Low, Medium, High }
    public enum ParseErrorType { None, Message, Warning, Error }
    public class ParseError
    {
        public string Message { get; internal set; }
        public ParserErrorLevel Level { get; internal set; }
        public ParseErrorType Type { get; internal set; }
        public int Line { get; internal set; }
        public int Character { get; internal set; }

        public ParseError(string message, ParserErrorLevel level, ParseErrorType type, int line, int character)
        {
            Message = message;
            Level = level;
            Type = type;
            Line = line;
            Character = character;
        }

        public ParseError(string message, ParserErrorLevel level, ParseErrorType type)
        {
            Message = message;
            Level = level;
            Type = type;
            Line = -1;
        }
    }
}
