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
        public int Start { get; set; }
        public int End { get; set; }

        public string Content { get; set; }
        public List<ASTNode> Children { get; set; }

        public ASTNode(int start, int end, string content, List<ASTNode> children)
        {
            Line = -1;
            Start = start;
            End = end;
            Content = content;
            Children = children;
        }

        public ASTNode(string content, List<ASTNode> children)
        {
            Content = content;
            Children = children;
        }

        public ASTNode(ASTNode other)
        {
            Line = other.Line;
            Start = other.Start;
            End = other.End;
            Content = other.Content;
            Children = new List<ASTNode>();
        }

        public ASTNode(int start, int end, string content)
        {
            Line = -1;
            Start = start;
            End = end;
            Content = content;
            Children = new List<ASTNode>();
        }

        public ASTNode(int start, int end, int line, string content)
        {
            Line = line;
            Start = start;
            End = end;
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
