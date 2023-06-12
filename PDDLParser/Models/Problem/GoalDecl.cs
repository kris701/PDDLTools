using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class GoalDecl : BaseNode, IDecl
    {
        public IExp GoalExp { get; set; }

        public GoalDecl(ASTNode node, IExp goalExp) : base(node)
        {
            GoalExp = goalExp;
        }

        public override string ToString()
        {
            return $"(:goal {GoalExp})";
        }
    }
}
