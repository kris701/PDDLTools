﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class PredicateExp : BaseWalkableNode, IExp, ICloneable, INamedNode
    {
        public string Name { get; set; }
        public List<NameExp> Arguments { get; set; }

        public PredicateExp(ASTNode node, INode parent, string name, List<NameExp> arguments) : base(node, parent)
        {
            Name = name;
            Arguments = arguments;
        }

        public override string ToString()
        {
            var paramRetStr = "";
            foreach (var arg in Arguments)
                paramRetStr += $" {arg}";
            return $"({Name}{paramRetStr})";
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode() + base.GetHashCode();
            foreach(var arg in Arguments)
                hash *= arg.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is PredicateExp exp) 
            { 
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public object Clone()
        {
            var newPredicateExp = new PredicateExp(new ASTNode(Start, End, Line), Parent, Name, new List<NameExp>());
            foreach (var arg in Arguments)
                newPredicateExp.Arguments.Add(new NameExp(new ASTNode(arg.Start, arg.End, arg.Line), newPredicateExp, arg.Name));
            return newPredicateExp;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            if (Name == name)
                res.Add(this);
            foreach (var arg in Arguments)
                res.AddRange(arg.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var arg in Arguments)
                res.AddRange(arg.FindTypes<T>());
            return res;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }
    }
}
