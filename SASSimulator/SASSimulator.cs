using PDDLParser.Models;
using PDDLParser.Models.Domain;
using SASSimulator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator
{
    public class SASSimulator : ISASSimulator
    {
        public PDDLDecl PDDL { get; }
        public List<ActionChoice> Plan { get; }
        public List<PredicateExp> State { get; }
        public int PlanStep { get; internal set; }

        public SASSimulator(PDDLDecl pDDL, List<ActionChoice> plan)
        {
            PDDL = pDDL;
            Plan = plan;
            PlanStep = 0;

            State = new List<PredicateExp>();
            foreach (var init in pDDL.Problem.Init.Predicates)
            {
                if (init is PredicateExp exp)
                {
                    var name = exp.Name;
                    var args = new List<NameExp>();
                    foreach (var arg in exp.Arguments)
                        args.Add(new NameExp(null, null, arg.Name));
                    State.Add(new PredicateExp(null, null, name, args));
                }
            }
        }

        public void Step(int steps = 1)
        {
            if (steps > 1)
                Step(steps - 1);

            if (PlanStep >= Plan.Count)
                return;

            ActionDecl acc = FindAction(Plan[PlanStep].Name);
            var args = BuildArgs(acc, Plan[PlanStep].Arguments);
            ApplyEffect(acc.Effects, args);
            PlanStep++;
        }

        private ActionDecl FindAction(string name)
        {
            foreach (var action in PDDL.Domain.Actions)
                if (action.Name == Plan[PlanStep].Name)
                    return action;
            throw new Exception($"Action name not found: {name}");
        }

        private Dictionary<string, string> BuildArgs(ActionDecl acc, List<string> args)
        {
            Dictionary<string, string> retDict = new Dictionary<string, string>();

            for (int i = 0; i < acc.Parameters.Values.Count; i++)
                retDict.Add(acc.Parameters.Values[i].Name, args[i]);

            return retDict;
        }

        private void ApplyEffect(IExp exp, Dictionary<string, string> args, bool inverse = false)
        {
            if (exp is AndExp and)
                foreach (var child in and.Children)
                    ApplyEffect(child, args);
            else if (exp is NotExp not)
                ApplyEffect(not.Child, args, true);
            else if (!State.Contains(exp))
            {
                if (exp is PredicateExp pred)
                {
                    if (inverse)
                    {
                        var gPred = GetGroundedPredicate(pred, args);
                        if (State.Contains(gPred))
                            State.Remove(gPred);
                    }
                    else
                    {
                        if (!State.Contains(pred))
                            State.Add(GetGroundedPredicate(pred, args));
                    }
                }

            }
        }

        private PredicateExp GetGroundedPredicate(PredicateExp liftedPred, Dictionary<string, string> args)
        {
            var groundedArgs = new List<NameExp>();
            foreach (var arg in liftedPred.Arguments)
                groundedArgs.Add(new NameExp(null, null, args[arg.Name]));
            return new PredicateExp(null, null, liftedPred.Name, groundedArgs);
        }
    }
}
