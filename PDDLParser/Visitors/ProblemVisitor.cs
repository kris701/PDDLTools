using PDDLParser.AST;
using PDDLParser.Listener;
using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Visitors
{
    public static class ProblemVisitor
    {
        public static IDecl Visit(ASTNode node, IErrorListener listener)
        {
            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.Content}",
                ParserErrorLevel.High,
                ParseErrorType.Error));
            return default;
        }

        private static string PurgeEscapeChars(string str) => str.Replace("\r", "").Replace("\n", "").Replace("\t", "");
    }
}
