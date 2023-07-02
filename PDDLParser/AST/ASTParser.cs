using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public class ASTParser
    {
        public IErrorListener Listener { get; }

        public ASTParser(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Parse(string text)
        {
            text = text.Replace(" - ", ASTTokens.TypeToken);
            var node = ParseAsNodeRec(text, text.IndexOf("("), text.LastIndexOf(")") + 2);
            SetLineNumberByCharacterNumberRec(text, node);
            return node;
        }

        private ASTNode ParseAsNodeRec(string text, int thisStart, int thisEnd)
        {
            if (text.Count(x => x == ')') > 1)
            {
                int thisP = text.IndexOf("(");

                var children = new List<ASTNode>();
                while (text.Count(x => x == ')' || x == '(') > 2)
                {
                    int currentLevel = 0;
                    int startP = text.IndexOf("(", thisP + 1);
                    int endP = text.Length;
                    for (int i = startP + 1; i < text.Length; i++)
                    {
                        if (text[i] == '(')
                            currentLevel++;
                        if (text[i] == ')')
                        {
                            if (currentLevel == 0)
                            {
                                endP = i;
                                break;
                            }
                            currentLevel--;
                        }
                    }

                    children.Add(ParseAsNodeRec(text.Substring(startP, endP - startP + 1), thisStart + startP, thisStart + endP));
                    text = ReplaceRangeWithSpaces(text, startP, endP);
                }
                var newText = text.Replace("(", "").Replace(")", "").Trim();
                return new ASTNode(
                    thisStart + 1,
                    thisEnd + 2,
                    newText,
                    children);
            }
            else
            {
                var newText = text.Replace("(", "").Replace(")", "").Trim();
                return new ASTNode(
                    thisStart + 1,
                    thisEnd + 2,
                    newText);
            }
        }

        private string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            var newText = text.Substring(0, from);
            newText += new string(' ', to - from + 1);
            newText += text.Substring(to + 1);
            return newText;
        }

        private void SetLineNumberByCharacterNumberRec(string source, ASTNode node)
        {
            foreach (var child in node.Children)
                SetLineNumberByCharacterNumberRec(source, child);
            var partStr = source.Substring(0, node.Start);
            node.Line = partStr.Count(c => c == ASTTokens.BreakToken) + 1;
        }
    }
}
