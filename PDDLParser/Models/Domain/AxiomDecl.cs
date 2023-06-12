using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class AxiomDecl : IDecl
    {
        public List<NameExp> Vars { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(List<NameExp> vars, IExp context, IExp implies)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }
    }
}
