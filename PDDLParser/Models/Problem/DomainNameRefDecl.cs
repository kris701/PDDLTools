using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class DomainNameRefDecl : IDecl
    {
        public string Name { get; set; }

        public DomainNameRefDecl(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(:domain {Name})";
        }
    }
}
