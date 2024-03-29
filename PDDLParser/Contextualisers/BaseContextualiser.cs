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
    public abstract class BaseContextualiser<T> : IContextualiser<T>
    {
        public abstract void Contexturalise(T decl, IErrorListener listener);

        internal int GetPredicateCountInExp(IExp exp)
        {
            if (exp is AndExp and)
            {
                int count = 0;
                foreach (var child in and.Children)
                    count += GetPredicateCountInExp(child);
                return count;
            }
            else if (exp is NotExp not)
            {
                return GetPredicateCountInExp(not.Child);
            }
            else if (exp is OrExp or)
            {
                return GetPredicateCountInExp(or.Option1) + GetPredicateCountInExp(or.Option2);
            }
            else
            {
                if (exp is PredicateExp)
                    return 1;
            }
            return 0;
        }

        internal void GetPredicatesInExp(IExp exp, List<PredicateExp> truePredicates, List<PredicateExp> falsePredicates, bool isNegated = false)
        {
            if (exp is AndExp and)
                foreach (var child in and.Children)
                    GetPredicatesInExp(child, truePredicates, falsePredicates, isNegated);
            else if (exp is NotExp not)
                GetPredicatesInExp(not.Child, truePredicates, falsePredicates, !isNegated);
            else if (exp is OrExp or)
            {
                GetPredicatesInExp(or.Option1, truePredicates, falsePredicates, isNegated);
                GetPredicatesInExp(or.Option2, truePredicates, falsePredicates, isNegated);
            }
            else
            {
                if (exp is PredicateExp pred)
                {
                    if (isNegated)
                        falsePredicates.Add(pred.Clone() as PredicateExp);
                    else
                        truePredicates.Add(pred.Clone() as PredicateExp);
                }
            }
        }

        internal bool DoesExpContainNodeType<U>(IExp exp)
        {
            if (exp is AndExp and)
            {
                if (typeof(U) == typeof(AndExp))
                    return true;

                foreach (var child in and.Children)
                    if (DoesExpContainNodeType<U>(child))
                        return true;
            }
            else if (exp is NotExp not)
            {
                if (typeof(U) == typeof(NotExp))
                    return true;

                return DoesExpContainNodeType<U>(not.Child);
            }
            else if (exp is OrExp or)
            {
                if (typeof(U) == typeof(OrExp))
                    return true;

                if (DoesExpContainNodeType<U>(or.Option1))
                    return true;
                return DoesExpContainNodeType<U>(or.Option2);
            }
            else if (exp is PredicateExp pred)
            {
                if (typeof(U) == typeof(PredicateExp))
                    return true;

                foreach (var arg in pred.Arguments)
                    if (DoesExpContainNodeType<U>(arg))
                        return true;
            }
            else if (exp is NameExp name)
            {
                if (typeof(U) == typeof(NameExp))
                    return true;
            }
            return false;
        }

        internal void ReplaceNameExpTypeWith(IExp node, NameExp with)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    ReplaceNameExpTypeWith(child, with);
            }
            else if (node is OrExp or)
            {
                ReplaceNameExpTypeWith(or.Option1, with);
                ReplaceNameExpTypeWith(or.Option2, with);
            }
            else if (node is NotExp not)
            {
                ReplaceNameExpTypeWith(not.Child, with);
            }
            else if (node is PredicateExp pred)
            {
                for (int i = 0; i < pred.Arguments.Count; i++)
                {
                    if (pred.Arguments[i].Name == with.Name)
                    {
                        pred.Arguments[i].Type = with.Type;
                    }
                }
            }
        }
    }
}
