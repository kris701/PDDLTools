﻿using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class MetricDecl : BaseWalkableNode, IDecl
    {
        public IExp MetricExp { get; set; }
        public string MetricType { get; set; }

        public MetricDecl(ASTNode node, INode parent, string metricType, IExp metricExp) : base(node, parent)
        {
            MetricType = metricType;
            MetricExp = metricExp;
        }

        public override string ToString()
        {
            return $"(:metric {MetricType} {MetricExp})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            return MetricExp.FindNames(name);
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(MetricExp.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + MetricExp.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is MetricDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return MetricExp;
        }
    }
}
