﻿using PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Domain
{
    public class DomainDecl : IDecl
    {
        public DomainNameDecl Name { get; set; }
        public RequirementsDecl Requirements { get; set; }
        public TypesDecl Types { get; set; }
        public ConstantsDecl Constants { get; set; }
        public PredicatesDecl Predicates { get; set; }
        public List<ActionDecl> Actions { get; set; }
    }
}
