using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class DurativeActionDecl : BaseNode, IDecl, INamedNode
    {
        public string Name { get; set; }

        public List<NameExp> Parameters { get; set; }
        public NameExp GetParameter(string name) => Parameters.SingleOrDefault(x => x.Name == name);
        public IExp Condition { get; set; }
        public IExp Effects { get; set; }
        public IExp Duration { get; set; }

        public DurativeActionDecl(ASTNode node, INode parent, string name, List<NameExp> parameters, IExp condition, IExp effects, IExp duration) : base(node, parent)
        {
            Name = name;
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            if (Name == name)
                res.Add(this);
            foreach (var param in Parameters)
                res.AddRange(param.FindNames(name));
            res.AddRange(Condition.FindNames(name));
            res.AddRange(Effects.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var param in Parameters)
                res.AddRange(param.FindTypes<T>());
            res.AddRange(Condition.FindTypes<T>());
            res.AddRange(Effects.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Name.GetHashCode();
            foreach (var param in Parameters)
                hash *= param.GetHashCode();
            hash *= Condition.GetHashCode();
            hash *= Effects.GetHashCode();
            hash *= Duration.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is DurativeActionDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
