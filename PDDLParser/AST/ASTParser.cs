using PDDLParser.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public class ASTParser
    {
        public IErrorListener Listener { get; }

        private string _currentSource = "";
        private int _currentCharacter = -1;

        public ASTParser(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Parse(string text)
        {
            _currentSource = text;
            var node = ParseAsNodeRec(text);
            SetLineNumberByCharacterNumberRec(text, node);
            return node;
        }

        private ASTNode ParseAsNodeRec(string text)
        {
            _currentCharacter = _currentSource.IndexOf("(", _currentCharacter + 1);
            int thisCharacter = _currentCharacter;

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

                    children.Add(ParseAsNodeRec(text.Substring(startP, endP - startP + 1)));
                    text = text.Remove(startP, endP - startP + 1);
                }
                var newText = text.Replace("(", "").Replace(")", "").Trim();
                return new ASTNode(
                    thisCharacter + 1,
                    newText,
                    children);
            }
            else
            {
                var newText = text.Replace("(", "").Replace(")", "").Trim();
                return new ASTNode(
                    thisCharacter + 1,
                    newText);
            }
        }

        private void SetLineNumberByCharacterNumberRec(string source, ASTNode node)
        {
            foreach (var child in node.Children)
                SetLineNumberByCharacterNumberRec(source, child);
            var partStr = source.Substring(0, node.Character);
            node.Line = partStr.Count(c => c == '\n') + 1;
        }
    }
}
