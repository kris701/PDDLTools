using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class ProblemDecl : BaseNode, IDecl
    {
        public ProblemNameDecl Name { get; set; }
        public DomainNameRefDecl DomainName { get; set; }
        public ObjectsDecl Objects { get; set; }
        public InitDecl Init { get; set; }
        public GoalDecl Goal { get; set; }

        public ProblemDecl(ASTNode node) : base(node, null) { }

        public override HashSet<INode> FindNames(string name)
        {
            HashSet<INode> res = new HashSet<INode>();

            if (Name != null)
                res.AddRange(Name.FindNames(name));
            if (DomainName != null)
                res.AddRange(DomainName.FindNames(name));
            if (Objects != null)
                res.AddRange(Objects.FindNames(name));
            if (Init != null)
                res.AddRange(Init.FindNames(name));
            if (Goal != null)
                res.AddRange(Goal.FindNames(name));

            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            if (Name != null)
                hash *= Name.GetHashCode();
            if (DomainName != null)
                hash *= DomainName.GetHashCode();
            if (Objects != null)
                hash *= Objects.GetHashCode();
            if (Init != null)
                hash *= Init.GetHashCode();
            if (Goal != null)
                hash *= Goal.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ProblemDecl exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
