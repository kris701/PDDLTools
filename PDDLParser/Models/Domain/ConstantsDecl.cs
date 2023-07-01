using PDDLParser.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLParser.Models.Domain
{
    public class ConstantsDecl : BaseNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, List<NameExp> constants) : base(node) 
        {
            Constants = constants;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Constants)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:constants{retStr})";
        }

        public override List<INode> FindName(string name)
        {
            List<INode> res = new List<INode>();
            foreach (var cons in Constants)
                res.AddRange(cons.FindName(name));
            return res;
        }
    }
}
