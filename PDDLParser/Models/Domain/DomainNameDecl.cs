using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Domain
{
    public class DomainNameDecl : IDecl
    {
        public string Name { get; set; }

        public DomainNameDecl(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"(domain {Name})";
        }
    }
}
