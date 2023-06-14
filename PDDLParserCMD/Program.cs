//using FastDownwardRunner;
//using PDDLParser;
//using PDDLParser.Models;
//using SASSimulator;
//using System.Numerics;

//IPDDLParser parser = new PDDLParser.PDDLParser();
//var pddlDecl = parser.ParseDomainAndProblemFiles(
//    "C:\\Users\\kris7\\Downloads\\domain.pddl",
//    "C:\\Users\\kris7\\Downloads\\problem.pddl");

//IRunner runner = new FDRunner(
//    "D:\\Program Files (x86)\\FastDownward\\downward",
//    "python",
//    999
//    );

//var res = runner.RunAsync(
//    "C:\\Users\\kris7\\Downloads\\domain.pddl",
//    "C:\\Users\\kris7\\Downloads\\problem.pddl",
//    "astar(blind())");

//IPlanParser planParser = new PlanParser();
//ISASSimulator sim = new SASSimulator.SASSimulator(
//    pddlDecl,
//    planParser.ParsePlanFile(Path.Combine(runner.FastDownwardFolder, "sas_plan")));


//var totalGoal = TotalGoalCount(pddlDecl.Problem.Goal.GoalExp);

//for (int i = 0; i < sim.Plan.Count; i++)
//{
//    var goalCount = GoalStateCount(pddlDecl.Problem.Goal.GoalExp, sim.State);
//    bool isGoal = goalCount == totalGoal;
//    bool isPartialGoal = goalCount > 0;


//    sim.Step();
//}

//Console.WriteLine("a");

//int TotalGoalCount(IExp exp)
//{
//    if (exp is AndExp and)
//    {
//        int count = 0;
//        foreach (var child in and.Children)
//            count += TotalGoalCount(child);
//        return count;
//    }
//    else if (exp is NotExp not)
//    {
//        return TotalGoalCount(not.Child);
//    }
//    return 1;
//}

//int GoalStateCount(IExp exp, List<PredicateExp> state, bool inverse = false)
//{
//    if (exp is AndExp and)
//    {
//        int count = 0;
//        foreach (var child in and.Children)
//            count += GoalStateCount(child, state);
//        return count;
//    }
//    else if (exp is NotExp not)
//    {
//        return GoalStateCount(not.Child, state, true);
//    }
//    else
//    {
//        if (exp is PredicateExp pred)
//        {
//            if (inverse)
//            {
//                if (!state.Contains(pred))
//                    return 1;
//            }
//            else
//            {
//                if (state.Contains(pred))
//                    return 1;
//            }
//        }
//    }
//    return 0;
//}


using System.Drawing;

GenerateSuitableLocations(500, 500, 10, 50);

List<Point> GenerateSuitableLocations(int width, int height, int count, int radius)
{
    Random rnd = new Random();
    List<Point> points = new List<Point>();
    points.Add(new Point(
                rnd.Next(0, width),
                rnd.Next(0, height)
                ));
    for (int i = 0; i < count - 1; i++)
    {
        var newPoint = new Point();
        double dist = double.MaxValue;
        newPoint = new Point(
                rnd.Next(0, width),
                rnd.Next(0, height)
                );
        while (GetClosestPointDistance(points, newPoint) < radius * 3)
            newPoint = new Point(
                rnd.Next(0, width),
                rnd.Next(0, height)
                );

        points.Add(newPoint);
    }

    bool changed = true;
    while (changed)
    {
        changed = false;
        for (int i = 0; i < points.Count - 1; i++)
        {
            var dist = Distance(points[i], points[i + 1]);
            for (int j = i + 2; j < points.Count; j++)
            {
                var newDist = Distance(points[i], points[j]);
                if (newDist < dist)
                {
                    var point1 = points[i + 1];
                    var point2 = points[j];
                    points[j] = point1;
                    points[i + 1] = point2;
                    changed = true;
                    break;
                }
            }
        }
    }

    return points;
}

double GetClosestPointDistance(List<Point> points, Point target)
{
    double dist = double.MaxValue;
    foreach (var point in points)
    {
        var newDist = Distance(point, target);
        if (newDist < dist)
            dist = newDist;
    }
    return dist;
}

double Distance(Point a, Point b) => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));