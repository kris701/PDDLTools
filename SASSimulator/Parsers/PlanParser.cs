using SASSimulator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator.Parsers
{
    public class PlanParser : IPlanParser
    {
        public List<ActionChoice> ParsePlanFile(string path) => ParsePlanText(File.ReadAllLines(path));
        public List<ActionChoice> ParsePlanText(string text) => ParsePlanText(text.Split('\n'));
        public List<ActionChoice> ParsePlanText(string[] text)
        {
            var plan = new List<ActionChoice>();
            foreach (var line in text)
            {
                if (!line.StartsWith(";") && line.Trim() != "")
                {
                    var innerLine = line.Replace("(", "").Replace(")", "");
                    var name = innerLine.Split(' ')[0];
                    var args = new List<string>();
                    foreach (var arg in innerLine.Split(' '))
                        if (arg.Trim() != name && arg.Trim() != "")
                            args.Add(arg.Trim());
                    plan.Add(new ActionChoice(name, args));
                }
            }
            return plan;
        }
    }
}
