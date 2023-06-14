﻿using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class NameExp : BaseNode, IExp, ICloneable
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public NameExp(ASTNode node, string name, string type) : base(node)
        {
            Name = name;
            Type = type;
        }

        public NameExp(ASTNode node, string name) : base(node) 
        {
            Name = name;
            Type = "";
        }

        public override string ToString()
        {
            if (Type == "")
                return $"({Name})";
            else
                return $"({Name} - {Type})";
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode() + Type.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is NameExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public object Clone()
        {
            return new NameExp(new ASTNode(Character, Line, ""), Name, Type);
        }
    }
}
