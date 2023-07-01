using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            if (Name == name)
                res.Add(this);
            foreach (var param in Parameters)
                res.AddRange(param.FindName(name));
            res.AddRange(Preconditions.FindName(name));
            res.AddRange(Effects.FindName(name));
            return res;
        }
    }
}
