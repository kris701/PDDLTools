using PDDLParser.Models;
using SASSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator
{
    public interface ISASSimulator
    {
        PDDLDecl PDDL { get; }
        List<PredicateExp> State { get; }

        List<ActionChoice> Plan { get; }
        int PlanStep { get; }

        void Step(int steps = 1);
    }
}
