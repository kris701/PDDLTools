using PDDLParser;

var parser = new Parser();

var domain = parser.ParseDomainFile("C:\\Users\\kris7\\Downloads\\domain2.pddl");
var problem = parser.ParseProblemFile("C:\\Users\\kris7\\Downloads\\problem2.pddl");

Console.WriteLine("a");