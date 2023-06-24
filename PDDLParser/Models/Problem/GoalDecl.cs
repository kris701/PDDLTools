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

        // Context
        public int PredicateCount { get; internal set; }
        public List<PredicateExp> TruePredicates { get; internal set; }
        public List<PredicateExp> FalsePredicates { get; internal set; }
        public bool DoesContainOr { get; internal set; }
        public bool DoesContainAnd { get; internal set; }
        public bool DoesContainNot { get; internal set; }
        public bool DoesContainPredicates { get; internal set; }
        public bool DoesContainNames { get; internal set; }

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
