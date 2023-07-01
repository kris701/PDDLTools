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
            Line = -1;
            Character = -1;
            if (node != null)
            {
                Line = node.Line;
                Character = node.Character;
            }
        }

        public abstract HashSet<INode> FindName(string name);

        public abstract bool Equals(object obj);

        public override int GetHashCode()
        {
            return Character.GetHashCode() + Line.GetHashCode();
        }
    }
}
