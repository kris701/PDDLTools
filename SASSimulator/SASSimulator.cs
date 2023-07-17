using PDDLParser.Models;
using PDDLParser.Models.Domain;
using SASSimulator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly:InternalsVisibleTo("SASSimulator.UnitTests")]

namespace SASSimulator
{
    public class SASSimulator : ISASSimulator
    {
        public PDDLDecl PDDL { get; }
        public List<ActionChoice> Plan { get; }
        public HashSet<Predicate> State { get; }
        public int PlanStep { get; internal set; }

        public SASSimulator(PDDLDecl pDDL, List<ActionChoice> plan)
        {
            PDDL = pDDL;
            Plan = plan;
            PlanStep = 0;

            State = new HashSet<Predicate>();
            if (pDDL.Problem.Init != null)
            {
                foreach (var init in pDDL.Problem.Init.Predicates)
                {
                    if (init is PredicateExp exp)
                    {
                        var newPred = new Predicate();
                        newPred.Name = exp.Name;
                        foreach (var arg in exp.Arguments)
                            newPred.Arguments.Add(arg.Name);
                        State.Add(newPred);
                    }
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

        internal ActionDecl FindAction(string name)
        {
            foreach (var action in PDDL.Domain.Actions)
                if (action.Name == name)
                    return action;
            throw new KeyNotFoundException($"Action name not found: {name}");
        }

        internal Dictionary<string, string> BuildArgs(ActionDecl acc, List<string> args)
        {
            Dictionary<string, string> retDict = new Dictionary<string, string>();

            for (int i = 0; i < acc.Parameters.Values.Count; i++)
                retDict.Add(acc.Parameters.Values[i].Name, args[i]);

            return retDict;
        }

        internal void ApplyEffect(INode exp, Dictionary<string, string> args, bool inverse = false)
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
                    if (!State.Contains(new Predicate(pred)))
                        State.Add(GetGroundedPredicate(pred, args));
                }
            }
            else if (exp is IWalkable walk)
            {
                if (walk is NotExp)
                    foreach (var child in walk)
                        ApplyEffect(child, args, true);
                else
                    foreach (var child in walk)
                        ApplyEffect(child, args);
            }
        }

        internal Predicate GetGroundedPredicate(PredicateExp liftedPred, Dictionary<string, string> args)
        {
            var grounded = new Predicate(liftedPred);
            for (int i = 0; i < liftedPred.Arguments.Count; i++)
                grounded.Arguments[i] = args[liftedPred.Arguments[i].Name];
            return grounded;
        }
    }
}
