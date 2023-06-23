using PDDLParser.Listener;
using PDDLParser.Models;
using PDDLParser.Models.Domain;
using PDDLParser.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser
{
    public interface IPDDLParser
    {
        IErrorListener Listener { get; }
        bool Contextualise { get; set; }

        PDDLDecl Parse(string domainFile = null, string problemFile = null);
    }
}
