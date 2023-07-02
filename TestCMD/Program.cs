using PDDLParser;

IPDDLParser parser = new PDDLParser.PDDLParser();
//var res = parser.Parse("C:\\Users\\kris7\\Downloads\\domain.pddl", "C:\\Users\\kris7\\Downloads\\problem.pddl");
var res = parser.Parse("C:\\Users\\kris7\\Downloads\\domain2.pddl", "C:\\Users\\kris7\\Downloads\\problem2.pddl");
Console.WriteLine("");