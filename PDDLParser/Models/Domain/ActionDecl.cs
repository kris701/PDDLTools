using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class ActionDecl : IDecl
    {
        public string Name { get; set; }

        public List<NameExp> Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }

        public ActionDecl(string name, List<NameExp> parameters, IExp preconditions, IExp effects)
        {
            Name = name;
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }
    }
}
