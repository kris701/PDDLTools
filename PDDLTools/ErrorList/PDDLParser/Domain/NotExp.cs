﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.ErrorList.PDDLParser.Domain
{
    public class NotExp : IExp
    {
        public IExp Child { get; set; }
    }
}
