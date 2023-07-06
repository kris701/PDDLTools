using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public class ASTParser : IASTParser<ASTNode>
    {
        public ASTNode Parse(string text)
        {
            text = TokenizeSpecials(text);
            var node = ParseAsNodeRec(text, 0, text.Length);
            SetLineNumberByCharacterNumberRec(text, node);
            return node;
        }

        public string TokenizeSpecials(string text)
        {
            text = text.Replace(" - ", ASTTokens.TypeToken);
            return text;
        }

        private ASTNode ParseAsNodeRec(string text, int thisStart, int thisEnd)
        {
            if (text.Contains('('))
            {
                var firstP = text.IndexOf("(");
                var lastP = text.LastIndexOf(")");
                var innerContent = ReplaceRangeWithSpaces(text, firstP, firstP + 1);
                innerContent = ReplaceRangeWithSpaces(innerContent, lastP, lastP + 1);

                var children = new List<ASTNode>();
                while (innerContent.Count(x => x == ')' || x == '(') >= 2)
                {
                    int currentLevel = 0;
                    int startP = innerContent.IndexOf("(");
                    int endP = innerContent.Length;
                    for (int i = startP + 1; i < innerContent.Length; i++)
                    {
                        if (innerContent[i] == '(')
                            currentLevel++;
                        if (innerContent[i] == ')')
                        {
                            if (currentLevel == 0)
                            {
                                endP = i + 1;
                                break;
                            }
                            currentLevel--;
                        }
                    }

                    var newContent = innerContent.Substring(startP, endP - startP);
                    children.Add(ParseAsNodeRec(newContent, thisStart + startP, thisStart + endP));
                    innerContent = ReplaceRangeWithSpaces(innerContent, startP, endP);
                }
                var outer = $"({innerContent.Trim()})";
                return new ASTNode(
                    thisStart,
                    thisEnd,
                    outer,
                    innerContent.Trim(),
                    children);
            }
            else
            {
                var newText = text.Trim();
                return new ASTNode(
                    thisStart,
                    thisEnd,
                    newText,
                    newText);
            }
        }

        private string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            var newText = text.Substring(0, from);
            newText += new string(' ', to - from);
            newText += text.Substring(to);
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
