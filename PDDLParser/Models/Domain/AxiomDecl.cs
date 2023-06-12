using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class AxiomDecl : BaseNode, IDecl
    {
        public List<NameExp> Vars { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, List<NameExp> vars, IExp context, IExp implies) : base(node)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }
    }
}
