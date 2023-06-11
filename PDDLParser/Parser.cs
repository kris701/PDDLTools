using PDDLParser.AST;
using PDDLParser.Domain;
using PDDLParser.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser
{
    public class Parser
    {
        public DomainFile ParseDomainFile(string parseFile)
        {
            string text = File.ReadAllText(parseFile);
            text = text.ToLower();
            CheckParenthesesMissmatch(text);

            var absAST = ASTParse(text);

            string name = "";
            List<string> requirements = new List<string>();
            List<TypeDefinition> types = new List<TypeDefinition>();
            List<NameNode> constants = new List<NameNode>();
            List<Predicate> predicates = new List<Predicate>();
            List<Action> actions = new List<Action>();

            return new DomainFile(
                name,
                requirements,
                types,
                constants,
                predicates,
                actions
                );
        }

        private ASTNode ASTParse(string text)
        {
            var children = new List<ASTNode>();

            while (text.Contains("(") && text.Contains(")"))
            {
                int firstP = text.IndexOf("(");
                int nextP = text.IndexOf("(", firstP + 1);
                int nextE = text.IndexOf(")");
                if (nextE > nextP || nextP != -1)
                {
                    children.Add(ASTParse(text.Substring(nextP + 1, nextE - nextP - 1)));
                    text = text.Remove(nextP + 1, nextE - nextP);
                }
            }

            return new ASTNode(text, children);
        }

        private void CheckParenthesesMissmatch(string text)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                throw new ParseException(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParserErrorLevel.High,
                    ParseErrorCategory.Error,
                    - 1
                    );
            }
        }
    }
}
