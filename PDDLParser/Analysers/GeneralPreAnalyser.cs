using PDDLParser.Listener;
using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PDDLParser.Analysers
{
    public class GeneralPreAnalyser : IAnalyser<string>
    {
        public void PostAnalyse(string decl, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        public void PreAnalyse(string text, IErrorListener listener)
        {
            CheckParenthesesMissmatch(text, listener);
            CheckForCasing(text, listener);
            CheckForUnsupportedRequirements(text, listener);
        }

        private void CheckParenthesesMissmatch(string text, IErrorListener listener)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                listener.AddError(new ParseError(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing,
                    ParserErrorCode.ParenthesesMissmatch));
            }
        }

        private void CheckForCasing(string text, IErrorListener listener)
        {
            if (text.Any(char.IsUpper))
            {
                listener.AddError(new ParseError(
                    $"Upper cased letters are ignored in PDDL",
                    ParseErrorType.Message,
                    ParseErrorLevel.PreParsing,
                    ParserErrorCode.UpperCaseLettersAreIgnored));
            }
        }

        private void CheckForUnsupportedRequirements(string text, IErrorListener listener)
        {
            foreach(var unsuportedPackage in PDDLParser.UnsupportedPackages)
            {
                if (text.Contains(unsuportedPackage))
                {
                    listener.AddError(new ParseError(
                        $"The reqirement '{unsuportedPackage}' is not supported by this parser. Results may not be accurate!",
                        ParseErrorType.Warning,
                        ParseErrorLevel.PreParsing,
                        ParserErrorCode.UnsupportedPackagesUsed));
                }
            }
        }
    }
}
