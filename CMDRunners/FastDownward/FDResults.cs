using CMDRunners.Helpers;
using CMDRunners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMDRunners.FastDownward
{
    public enum FDExitCode
    {
        SUCCESS = 0,
        SEARCH_PLAN_FOUND_AND_OUT_OF_MEMORY = 1,
        SEARCH_PLAN_FOUND_AND_OUT_OF_TIME = 2,
        SEARCH_PLAN_FOUND_AND_OUT_OF_MEMORY_AND_TIME = 3,

        TRANSLATE_UNSOLVABLE = 10,
        SEARCH_UNSOLVABLE = 11,
        SEARCH_UNSOLVED_INCOMPLETE = 12,

        TRANSLATE_OUT_OF_MEMORY = 20,
        TRANSLATE_OUT_OF_TIME = 21,
        SEARCH_OUT_OF_MEMORY = 22,
        SEARCH_OUT_OF_TIME = 23,
        SEARCH_OUT_OF_MEMORY_AND_TIME = 24,

        TRANSLATE_CRITICAL_ERROR = 30,
        TRANSLATE_INPUT_ERROR = 31,
        SEARCH_CRITICAL_ERROR = 32,
        SEARCH_INPUT_ERROR = 33,
        SEARCH_UNSUPPORTED = 34,
        DRIVER_CRITICAL_ERROR = 35,
        DRIVER_INPUT_ERROR = 36,
        DRIVER_UNSUPPORTED = 37,
    }

    public class FDResults
    {
        public ProcessCompleteReson ResultReason { get; }
        public List<LogItem> Log { get; }

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

        public int PlanLength { get; set; }
        public int PlanCost { get; set; }
        public int ExpandedStates { get; set; }
        public int ReopenedStates { get; set; }
        public int EvaluatedStates { get; set; }
        public int Evaluations { get; set; }
        public int GeneratedStates { get; set; }
        public int DeadEndStates { get; set; }
        public int ExpandedUntilLastJumpStates { get; set; }
        public int ReopenedUntilLastJumpStates { get; set; }
        public int EvaluatedUntilLastJumpStates { get; set; }
        public int GeneratedUntilLastJumpStates { get; set; }
        public int NumberOfRegisteredStates { get; set; }
        public double IntHashSetLoadFactor { get; set; }
        public int IntHashSetResizes { get; set; }

        public double SearchTime { get; set; }
        public double TotalTime { get; set; }

        public FDExitCode ExitCode { get; set; }


        public FDResults(List<LogItem> log, ProcessCompleteReson resultReason) 
        {
            ResultReason = resultReason;

            Log = log;

            if (resultReason == ProcessCompleteReson.RanToCompletion)
            {
                foreach (var item in log)
                {
                    if (item.Type == LogItem.ItemType.Log)
                    {
                        var lowLine = item.Content.ToLower();

                        // Translator Vars
                        if (lowLine.Contains("translator variables: "))
                            TranslatorVariables = Convert.ToInt32(lowLine.Replace("translator variables: ", ""));
                        else if (lowLine.Contains("translator derived variables: "))
                            TranslatorDerivedVariables = Convert.ToInt32(lowLine.Replace("translator derived variables: ", ""));
                        else if (lowLine.Contains("translator facts: "))
                            TranslatorFacts = Convert.ToInt32(lowLine.Replace("translator facts: ", ""));
                        else if (lowLine.Contains("translator goal facts: "))
                            TranslatorGoalFacts = Convert.ToInt32(lowLine.Replace("translator goal facts: ", ""));
                        else if (lowLine.Contains("translator mutex groups: "))
                            TranslatorMutexGroups = Convert.ToInt32(lowLine.Replace("translator mutex groups: ", ""));
                        else if (lowLine.Contains("translator total mutex groups size: "))
                            TranslatorTotalMutexGroupsSize = Convert.ToInt32(lowLine.Replace("translator total mutex groups size: ", ""));
                        else if (lowLine.Contains("translator operators: "))
                            TranslatorOperators = Convert.ToInt32(lowLine.Replace("translator operators: ", ""));
                        else if (lowLine.Contains("translator axioms: "))
                            TranslatorAxioms = Convert.ToInt32(lowLine.Replace("translator axioms: ", ""));
                        else if (lowLine.Contains("translator task size: "))
                            TranslatorTaskSize = Convert.ToInt32(lowLine.Replace("translator task size: ", ""));

                        // Search Vars
                        var ignoreSquareBracketLine = lowLine.Substring(lowLine.IndexOf("]") + 1).Trim();

                        if (ignoreSquareBracketLine.StartsWith("plan length: ") && ignoreSquareBracketLine.Contains("step(s)."))
                            PlanLength = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("plan length: ") + "plan length: ".Length).Replace("step(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("plan cost: "))
                            PlanCost = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("plan cost: ") + "plan cost: ".Length));
                        else if (ignoreSquareBracketLine.StartsWith("expanded until last jump: ") && ignoreSquareBracketLine.Contains("state(s)."))
                            ExpandedUntilLastJumpStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("expanded until last jump: ") + "expanded until last jump: ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("reopened until last jump: ") && ignoreSquareBracketLine.Contains("state(s)."))
                            ReopenedUntilLastJumpStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("reopened until last jump: ") + "reopened until last jump: ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("evaluated until last jump: ") && ignoreSquareBracketLine.Contains("state(s)."))
                            EvaluatedUntilLastJumpStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("evaluated until last jump: ") + "evaluated until last jump: ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("generated until last jump: ") && ignoreSquareBracketLine.Contains("state(s)."))
                            GeneratedUntilLastJumpStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("generated until last jump: ") + "generated until last jump: ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("expanded ") && ignoreSquareBracketLine.Contains("state(s)."))
                            ExpandedStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("expanded ") + "expanded ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("reopened ") && ignoreSquareBracketLine.Contains("state(s)."))
                            ReopenedStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("reopened ") + "reopened ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("evaluated ") && ignoreSquareBracketLine.Contains("state(s)."))
                            EvaluatedStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("evaluated ") + "evaluated ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("evaluations: "))
                            Evaluations = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("evaluations: ") + "evaluations: ".Length));
                        else if (ignoreSquareBracketLine.StartsWith("generated ") && ignoreSquareBracketLine.Contains("state(s)."))
                            GeneratedStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("generated ") + "generated ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("dead ends: ") && ignoreSquareBracketLine.Contains("state(s)."))
                            DeadEndStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("dead ends: ") + "dead ends: ".Length).Replace("state(s).", ""));
                        else if (ignoreSquareBracketLine.StartsWith("number of registered states: "))
                            NumberOfRegisteredStates = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("number of registered states: ") + "number of registered states: ".Length));
                        else if (ignoreSquareBracketLine.StartsWith("int hash set load factor: "))
                            IntHashSetLoadFactor = Convert.ToDouble(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("=") + 1));
                        else if (ignoreSquareBracketLine.StartsWith("int hash set resizes: "))
                            IntHashSetResizes = Convert.ToInt32(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("int hash set resizes: ") + "int hash set resizes: ".Length));

                        else if (ignoreSquareBracketLine.StartsWith("search time: "))
                            SearchTime = Convert.ToDouble(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("search time: ") + "search time: ".Length).Replace("s", ""));
                        else if (ignoreSquareBracketLine.StartsWith("total time: "))
                            TotalTime = Convert.ToDouble(ignoreSquareBracketLine.Substring(ignoreSquareBracketLine.IndexOf("total time: ") + "total time: ".Length).Replace("s", ""));

                        // General Vars
                        if (lowLine.Contains("search exit code: "))
                            ExitCode = (FDExitCode)Convert.ToInt32(lowLine.Replace("search exit code: ", ""));
                        else if (lowLine.Contains("solution found."))
                            WasSolutionFound = true;
                        else if (lowLine.Contains("no solution found"))
                            WasSolutionFound = false;
                    }
                }
            }
        }
    }
}
