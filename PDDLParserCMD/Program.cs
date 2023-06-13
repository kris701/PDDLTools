using PDDLParser;

var parser = new PDDLParser.PDDLParser();

var domain = parser.ParseDomainFile("C:\\Users\\kris7\\Downloads\\domain.pddl");
var problem = parser.ParseProblemFile("C:\\Users\\kris7\\Downloads\\problem.pddl");

Console.WriteLine("a");