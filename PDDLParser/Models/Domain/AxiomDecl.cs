using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            foreach (var var in Vars)
                res.AddRange(var.FindName(name));
            res.AddRange(Context.FindName(name));
            res.AddRange(Implies.FindName(name));
            return res;
        }
    }
}
