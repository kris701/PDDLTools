﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TimelessDecl : BaseNode, IDecl
    {
        public List<NameExp> Items { get; set; }

        public TimelessDecl(ASTNode node, INode parent, List<NameExp> timeless) : base(node, parent)
        {
            Items = timeless;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Items)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:timeless{retStr})";
        }

        public override HashSet<INode> FindNames(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var item in Items)
                res.AddRange(item.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var item in Items)
                res.AddRange(item.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var item in Items)
                hash *= item.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimelessDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
