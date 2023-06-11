﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class PredicateStmt : IExp
    {
        public string Name { get; set; }
        public List<NameExp> Arguments { get; set; }

        public PredicateStmt(string name, List<NameExp> arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public override string ToString()
        {
            var paramRetStr = "";
            foreach(var arg in Arguments)
                paramRetStr += arg.ToString();
            return $"{Name} {paramRetStr}";
        }
    }
}
