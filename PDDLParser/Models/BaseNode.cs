using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public abstract class BaseNode : INode
    {
        public int Character { get; set; }
        public int Line { get; set; }

        public BaseNode(ASTNode node)
        {
            Line = node.Line;
            Character = node.Character;
        }
    }
}
