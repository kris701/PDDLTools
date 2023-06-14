using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models
{
    public class PredicateExp : BaseNode, IExp
    {
        public string Name { get; set; }
        public List<NameExp> Arguments { get; set; }

        public PredicateExp(ASTNode node, string name, List<NameExp> arguments) : base(node)
        {
            Name = name;
            Arguments = arguments;
        }

        public override string ToString()
        {
            var paramRetStr = "";
            foreach (var arg in Arguments)
                paramRetStr += $" {arg}";
            return $"({Name}{paramRetStr})";
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            foreach(var  arg in Arguments)
                hash *= arg.Name.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is PredicateExp exp) 
            { 
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }
    }
}
