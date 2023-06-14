using FastDownwardRunner;
using PDDLParser;
using SASSimulator;

IPDDLParser parser = new PDDLParser.PDDLParser();
var pddlDecl = parser.ParseDomainAndProblemFiles(
    "C:\\Users\\kris7\\Downloads\\domain.pddl",
    "C:\\Users\\kris7\\Downloads\\problem.pddl");

IRunner runner = new FDRunner(
    "D:\\Program Files (x86)\\FastDownward\\downward",
    "python",
    999
    );

var res = runner.RunAsync(
    "C:\\Users\\kris7\\Downloads\\domain.pddl",
    "C:\\Users\\kris7\\Downloads\\problem.pddl",
    "astar(blind())");

IPlanParser planParser = new PlanParser();
ISASSimulator sim = new SASSimulator.SASSimulator(
    pddlDecl,
    planParser.ParsePlanFile(Path.Combine(runner.FastDownwardFolder, "sas_plan")));

sim.Step(10);

Console.WriteLine("a");