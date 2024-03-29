﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class NotExp : BaseWalkableNode, IExp
    {
        public IExp Child { get; set; }

        public NotExp(ASTNode node, INode parent, IExp child) : base(node, parent)
        {
            Child = child;
        }

        public override string ToString()
        {
            return $"(not {Child})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            return Child.FindNames(name);
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(Child.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Child.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NotExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Child;
        }
    }
}
