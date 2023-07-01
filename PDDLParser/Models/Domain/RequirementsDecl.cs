using PDDLParser.AST;
using PDDLParser.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class RequirementsDecl : BaseNode, IDecl
    {
        public List<NameExp> Requirements {  get; set; }

        public RequirementsDecl(ASTNode node, List<NameExp> requirements) : base(node)
        {
            Requirements = requirements;
        }

        public override string ToString()
        {
            var reqStr = "";
            foreach (var requirement in Requirements)
                reqStr += $" {requirement}";
            return $"(:requirements{reqStr})";
        }

        public override HashSet<INode> FindName(string name)
        {
            HashSet<INode> res = new HashSet<INode>();
            foreach (var requirement in Requirements)
                res.AddRange(requirement.FindName(name));
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var req in Requirements)
                hash *= req.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is RequirementsDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }
    }
}
