﻿using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Contextualisers
{
    public class PDDLProblemDeclContextualiser : BaseContextualiser<ProblemDecl>
    {
        public override void Contexturalise(ProblemDecl decl, IErrorListener listener)
        {
            SetGoalContext(decl.Goal);
            SetInitContext(decl.Init);
            LinkObjects(decl);
        }

        private void SetGoalContext(GoalDecl goal)
        {
            if (goal.GoalExp != null)
            {
                goal.GoalExpCount = GetPredicateCountInExp(goal.GoalExp);

                List<PredicateExp> truePredicates = new List<PredicateExp>();
                List<PredicateExp> falsePredicates = new List<PredicateExp>();
                GetPredicatesInExp(goal.GoalExp, truePredicates, falsePredicates);
                goal.TruePredicates = truePredicates;
                goal.FalsePredicates = falsePredicates;

                goal.DoesContainAnd = DoesExpContainNodeType<AndExp>(goal.GoalExp);
                goal.DoesContainOr = DoesExpContainNodeType<OrExp>(goal.GoalExp);
                goal.DoesContainNot = DoesExpContainNodeType<NotExp>(goal.GoalExp);
                goal.DoesContainPredicates = DoesExpContainNodeType<PredicateExp>(goal.GoalExp);
                goal.DoesContainNames = DoesExpContainNodeType<NameExp>(goal.GoalExp);
            }
        }

        private void SetInitContext(InitDecl init)
        {

        }

        private void LinkObjects(ProblemDecl decl)
        {
            if (decl.Objects != null)
            {
                foreach (var obj in decl.Objects.Objs)
                {
                    if (decl.Init != null)
                    {
                        foreach(var init in decl.Init.Predicates)
                        {
                            for (int i = 0; i < init.Arguments.Count; i++)
                            {
                                if (init.Arguments[i].Name == obj.Name)
                                    init.Arguments[i] = obj;
                            }
                        }
                    }

                    if (decl.Goal != null)
                        ReplaceNameExpWith(decl.Goal.GoalExp, obj);
                }
            }
        }
    }
}
