using PDDLParser.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public abstract class BaseWalkableNode : BaseNode, IWalkable
    {
        protected BaseWalkableNode(ASTNode node, INode parent) : base(node, parent)
        {
        }

        public abstract IEnumerator<INode> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
