using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLTools.Models
{
    public class FDResults
    {
        public bool WasSolutionFound { get; set; }

        public int TranslatorVariables { get; set; }
        public int TranslatorDerivedVariables { get; set; }
        public int TranslatorFacts { get; set; }
        public int TranslatorGoalFacts { get; set; }
        public int TranslatorMutexGroups { get; set; }
        public int TranslatorTotalMutexGroupsSize { get; set; }
        public int TranslatorOperators { get; set; }
        public int TranslatorAxioms { get; set; }
        public int TranslatorTaskSize { get; set; }

        public FDResults(List<string> parseData, List<string> parseErrData) 
        { 
            foreach(string line in parseData)
            {
                var lowLine = line.ToLower();
                if (lowLine.Contains("translator variables: "))
                    TranslatorVariables = Convert.ToInt32(lowLine.Replace("translator variables: ", ""));
                if (lowLine.Contains("translator derived variables: "))
                    TranslatorDerivedVariables = Convert.ToInt32(lowLine.Replace("translator derived variables: ", ""));
                if (lowLine.Contains("translator facts: "))
                    TranslatorFacts = Convert.ToInt32(lowLine.Replace("translator facts: ", ""));
                if (lowLine.Contains("translator goal facts: "))
                    TranslatorGoalFacts = Convert.ToInt32(lowLine.Replace("translator goal facts: ", ""));
                if (lowLine.Contains("translator mutex groups: "))
                    TranslatorMutexGroups = Convert.ToInt32(lowLine.Replace("translator mutex groups: ", ""));
                if (lowLine.Contains("translator total mutex groups size: "))
                    TranslatorTotalMutexGroupsSize = Convert.ToInt32(lowLine.Replace("translator total mutex groups size: ", ""));
                if (lowLine.Contains("translator operators: "))
                    TranslatorOperators = Convert.ToInt32(lowLine.Replace("translator operators: ", ""));
                if (lowLine.Contains("translator axioms: "))
                    TranslatorAxioms = Convert.ToInt32(lowLine.Replace("translator axioms: ", ""));
                if (lowLine.Contains("translator task size: "))
                    TranslatorTaskSize = Convert.ToInt32(lowLine.Replace("translator task size: ", ""));

                if (lowLine.Contains("solution found"))
                    WasSolutionFound = true;
                if (lowLine.Contains("no solution found"))
                    WasSolutionFound = false;
            }
        }
    }
}
