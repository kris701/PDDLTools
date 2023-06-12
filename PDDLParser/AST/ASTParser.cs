using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public class ASTParser
    {
        public ASTNode ASTParse(string text)
        {
            if (text.Count(x => x == ')') > 1)
            {
                var children = new List<ASTNode>();

                int thisP = text.IndexOf("(");

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

                    children.Add(ASTParse(text.Substring(startP, endP - startP + 1)));
                    text = text.Remove(startP, endP - startP + 1);
                }
                return new ASTNode(
                    text.Replace("(", "").Replace(")", "").Trim(),
                    children);
            }
            else
            {
                return new ASTNode(
                    text.Replace("(", "").Replace(")", "").Trim());
            }
        }
    }
}
