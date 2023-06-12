﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Models.Problem
{
    public class ProblemDecl
    {
        public ProblemNameDecl Name { get; set; }
        public DomainNameRefDecl DomainName { get; set; }
        public ObjectsDecl Objects { get; set; }
        public InitDecl Init { get; set; }
        public GoalDecl Goal { get; set; }
    }
}