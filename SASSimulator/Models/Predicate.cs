using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator.Models
{
    public class Predicate : ICloneable
    {
        public string Name { get; set; }
        public List<string> Arguments { get; set; }

        public Predicate()
        {
            Arguments = new List<string>();
        }

        public Predicate(PredicateExp exp)
        {
            Name = exp.Name;
            Arguments = new List<string>();
            foreach (var arg in exp.Arguments)
                Arguments.Add(arg.Name);
        }

        public object Clone()
        {
            var clone = new Predicate();
            clone.Name = Name;
            foreach (var arg in Arguments)
                clone.Arguments.Add(arg);
            return clone;
        }

        public override string ToString()
        {
            string argStr = "";
            foreach (var arg in Arguments)
                argStr += $" {arg}";
            return $"({Name}{argStr})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Predicate pred)
            {
                if (pred.Name != Name)
                    return false;
                if (pred.Arguments.Count !=  Arguments.Count) 
                    return false;
                for (int i = 0; i < Arguments.Count; i++)
                    if (pred.Arguments[i] != Arguments[i])
                        return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = Name.GetHashCode();
            foreach (var arg in Arguments)
                hash *= arg.GetHashCode();
            return hash;
        }
    }
}
