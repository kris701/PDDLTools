using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Listener
{
    public enum ParseErrorType { None, Message, Warning, Error }
    public enum ParseErrorLevel { None, PreParsing, Parsing, Contexturaliser, Analyser }
    public class ParseError
    {
        public string Message { get; internal set; }
        public ParseErrorType Type { get; internal set; }
        public ParseErrorLevel Level { get; internal set; }
        public int Line { get; internal set; }
        public int Character { get; internal set; }

        public ParseError(string message, ParseErrorType type, ParseErrorLevel level, int line, int character)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = line;
            Character = character;
        }

        public ParseError(string message, ParseErrorType type, ParseErrorLevel level)
        {
            Message = message;
            Type = type;
            Level = level;
            Line = -1;
            Character = -1;
        }
    }
}
