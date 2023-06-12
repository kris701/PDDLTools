using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.AST
{
    public class ASTNode
    {
        public string Content { get; set; }
        public List<ASTNode> Children { get; set; }

        public ASTNode(string content, List<ASTNode> children)
        {
            Content = content;
            Children = children;
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
