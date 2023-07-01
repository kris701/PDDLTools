using PDDLParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Helpers
{
    public static class PDDLSearcher
    {
        public static INode GetNodeNearestPosition(PDDLDecl decl, int position)
        {
            INode lastValid = null;
            if (decl.Domain != null)
            {
                lastValid = decl.Domain;
                if (decl.Domain.Character < position)
                    return decl.Domain;
            }
            return null;
        }
    }
}
