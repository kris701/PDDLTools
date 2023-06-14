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
        private IExp _goalExp;
        public IExp GoalExp { 
            get 
            { 
                return _goalExp; 
            } set {
                _goalExp = value;
                GoalExpCount = GoalStateCount(value);
            } 
        }
        public int GoalExpCount { get; internal set; }

        public GoalDecl(ASTNode node, IExp goalExp) : base(node)
        {
            GoalExp = goalExp;
        }

        public override string ToString()
        {
            return $"(:goal {GoalExp})";
        }

        private int GoalStateCount(IExp exp)
        {
            if (exp is AndExp and)
            {
                int count = 0;
                foreach (var child in and.Children)
                    count += GoalStateCount(child);
                return count;
            }
            else if (exp is NotExp not)
            {
                return GoalStateCount(not.Child);
            }
            else
            {
                if (exp is PredicateExp pred)
                    return 1;
            }
            return 0;
        }
    }
}
