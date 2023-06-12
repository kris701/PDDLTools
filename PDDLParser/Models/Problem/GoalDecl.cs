using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class GoalDecl : IDecl
    {
        public IExp GoalExp { get; set; }

        public GoalDecl(IExp goalExp)
        {
            GoalExp = goalExp;
        }

        public override string ToString()
        {
            return $"(:goal {GoalExp})";
        }
    }
}
