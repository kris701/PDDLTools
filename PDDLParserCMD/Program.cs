using FastDownwardRunner;
using PDDLParser;
using PDDLParser.Models;
using SASSimulator;
using System.Numerics;

IPDDLParser parser = new PDDLParser.PDDLParser();
var pddlDecl = parser.ParseDomainAndProblemFiles(
    "C:\\Users\\kris7\\Downloads\\domain.pddl",
    "C:\\Users\\kris7\\Downloads\\problem.pddl");

Console.WriteLine("a");