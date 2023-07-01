using PDDLParser.AST;
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

        public ProblemDecl(ASTNode node) : base(node) { }

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();

            res.AddRange(Name.FindName(name));
            res.AddRange(DomainName.FindName(name));
            res.AddRange(Objects.FindName(name));
            res.AddRange(Init.FindName(name));
            res.AddRange(Goal.FindName(name));

            return res;
        }
    }
}
