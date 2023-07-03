using PDDLParser;
using PDDLParser.Models;

IPDDLParser parser = new PDDLParser.PDDLParser();
//var res = parser.Parse("C:\\Users\\kris7\\Downloads\\domain.pddl", "C:\\Users\\kris7\\Downloads\\problem.pddl");
var res = parser.Parse("C:\\Users\\kris7\\Downloads\\domain2.pddl", "C:\\Users\\kris7\\Downloads\\problem2.pddl");

var nodes = res.FindTypes<PredicateExp>();

Console.WriteLine("");