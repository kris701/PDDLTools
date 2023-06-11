﻿using PDDLTools.ErrorList.PDDLParser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace PDDLTools.ErrorList.PDDLParser.Domain
{
    public class DomainFile
    {
        public string Name { get; set; }
        public List<string> Requirements { get; set; }
        public List<TypeDefinition> Types { get; set; }
        public List<NameNode> Constants { get; set; }
        public List<Predicate> Predicates { get; set; }
        public List<Action> Actions { get; set; }

        public DomainFile(string parseFile)
        {
            var text = File.ReadAllText(parseFile);
            CheckParenthesesMissmatch(text);
        }

        private void CheckParenthesesMissmatch(string text)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                throw new ParseException(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!", 
                    TaskErrorCategory.Error,
                    -1, 
                    TaskPriority.High
                    );
            }
        }
    }
}
