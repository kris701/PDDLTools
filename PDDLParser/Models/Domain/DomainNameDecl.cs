using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class DomainNameDecl : BaseNode, IDecl
    {
        public string Name { get; set; }

        public DomainNameDecl(ASTNode node, string name) : base(node)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(domain {Name})";
        }
    }
}
