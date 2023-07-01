using PDDLParser.AST;
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

        public ActionDecl(ASTNode node, string name, List<NameExp> parameters, IExp preconditions, IExp effects) : base(node)
        {
            Name = name;
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            if (Name == name)
                res.Add(this);
            foreach (var param in Parameters)
                res.AddRange(param.FindName(name));
            res.AddRange(Preconditions.FindName(name));
            res.AddRange(Effects.FindName(name));
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
