﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class ActionDecl :  BaseNode, IDecl
    {
        public string Name { get; set; }

        public List<NameExp> Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }

        public ActionDecl(ASTNode node, INode parent, string name, List<NameExp> parameters, IExp preconditions, IExp effects) : base(node, parent)
        {
            Name = name;
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public override HashSet<INode> FindNames(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            if (Name == name)
                res.Add(this);
            foreach (var param in Parameters)
                res.AddRange(param.FindNames(name));
            res.AddRange(Preconditions.FindNames(name));
            res.AddRange(Effects.FindNames(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Name.GetHashCode();
            foreach (var param in Parameters)
                hash *= param.GetHashCode();
            hash *= Preconditions.GetHashCode();
            hash *= Effects.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ActionDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
