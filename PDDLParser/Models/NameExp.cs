﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using PDDLParser.Models.Domain;
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
        public TypeNameDecl Type { get; set; }

        public NameExp(ASTNode node, INode parent, string name, TypeNameDecl type) : base(node, parent)
        {
            Name = name;
            Type = type;
        }

        public NameExp(ASTNode node, INode parent, string name) : base(node, parent) 
        {
            Name = name;
            Type = new TypeNameDecl(node, this, "");
        }

        public override string ToString()
        {
            if (Type.Name == "")
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
            return new NameExp(new ASTNode(Start, Line, ""), Parent, Name, Type);
        }

        public override HashSet<INode> FindNames(string name)
        {
            var result = new HashSet<INode>();
            if (Name == name)
                result.Add(this);
            result.AddRange(Type.FindNames(name));
            return result;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(Type.FindTypes<T>());
            return res;
        }
    }
}
