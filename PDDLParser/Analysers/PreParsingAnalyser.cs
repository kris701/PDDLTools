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
            CheckForCasing(text, listener);
            CheckForUnsupportedRequirements(text, listener);
        }

        private static void CheckParenthesesMissmatch(string text, IErrorListener listener)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                listener.AddError(new ParseError(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParseErrorType.Error));
            }
        }

        private static void CheckForCasing(string text, IErrorListener listener)
        {
            if (text.Any(char.IsUpper))
            {
                listener.AddError(new ParseError(
                    $"Upper cased letters are ignored in PDDL",
                    ParseErrorType.Message));
            }
        }

        private static List<string> _unsupportedPackages = new List<string>()
        {
            ":existential-preconditions",
            ":adl",
            ":universal-preconditions",
            ":quantified-preconditions",
            ":conditional-effects",
            ":action-expansions",
            ":foreach-expansions",
            ":dag-expansions",
            ":subgoals-through-axioms",
            ":safety-constraints",
            ":expression-evaluation",
            ":fluents",
            ":open-world",
            ":true-negation",
            ":ucpop"
        };
        private static void CheckForUnsupportedRequirements(string text, IErrorListener listener)
        {
            foreach(var unsuportedPackage in _unsupportedPackages)
            {
                if (text.Contains(unsuportedPackage))
                {
                    listener.AddError(new ParseError(
                        $"The reqirement '{unsuportedPackage}' is not supported by this parser. Results may not be accurate!",
                        ParseErrorType.Warning));
                }
            }
        }
    }
}
