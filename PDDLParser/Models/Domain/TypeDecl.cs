﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class TypeDecl : BaseNode, IDecl
    {
        public TypeNameDecl TypeName { get; set; }
        public List<TypeNameDecl> SubTypes { get; set; }

        public TypeDecl(ASTNode node, TypeNameDecl name, List<TypeNameDecl> subTypes) : base(node)
        {
            TypeName = name;
            SubTypes = subTypes;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var subType in SubTypes)
                retStr += $"{subType} ";
            return $"{retStr} - {TypeName}";
        }

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            res.AddRange(TypeName.FindName(name));
            foreach (var subtype in SubTypes)
                res.AddRange(subtype.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode() + TypeName.GetHashCode();
            foreach (var type in SubTypes)
                hash *= type.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TypeDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
