using SASSimulator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SASSimulator
{
    public interface IPlanParser
    {
        List<ActionChoice> ParsePlanFile(string path);
        List<ActionChoice> ParsePlanText(string text);
        List<ActionChoice> ParsePlanText(string[] text);
    }
}
