using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public class ASTNode
    {
        public int Line { get; set; }
        public int Character { get; set; }

        public string Content { get; set; }
        public List<ASTNode> Children { get; set; }

        public ASTNode(int character, string content, List<ASTNode> children)
        {
            Line = -1;
            Character = character;
            Content = content;
            Children = children;
        }

        public ASTNode(string content, List<ASTNode> children)
        {
            Content = content;
            Children = children;
        }


        public ASTNode(int character, string content)
        {
            Line = -1;
            Character = character;
            Content = content;
            Children = new List<ASTNode>();
        }

        public ASTNode(string content)
        {
            Content = content;
            Children = new List<ASTNode>();
        }

        public override string ToString()
        {
            return Content;
        }
    }
}
