using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public static class PreParsingAnalyser
    {
        public static void AnalyseText(string text, IErrorListener listener)
        {
            CheckParenthesesMissmatch(text, listener);
        }

        private static void CheckParenthesesMissmatch(string text, IErrorListener listener)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                listener.AddError(new ParseError(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParserErrorLevel.High,
                    ParseErrorType.Error));
            }
        }
    }
}
