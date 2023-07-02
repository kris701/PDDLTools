﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Models.Domain
{
    public class AxiomDecl : BaseNode, IDecl
    {
        public List<NameExp> Vars { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, INode parent, List<NameExp> vars, IExp context, IExp implies) : base(node, parent)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public override HashSet<INode> FindNames(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var var in Vars)
                res.AddRange(var.FindNames(name));
            res.AddRange(Context.FindNames(name));
            res.AddRange(Implies.FindNames(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var var in Vars)
                hash *= var.GetHashCode();
            hash *= Context.GetHashCode();
            hash *= Implies.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is AxiomDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
