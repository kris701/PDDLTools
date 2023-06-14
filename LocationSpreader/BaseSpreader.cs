using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocationSpreader
{
    public abstract class BaseSpreader : ILocationSpreader
    {
        public abstract List<Point> GenerateSuitableLocations(int width, int height, int count, int radius);

        internal double GetClosestPointDistance(List<Point> points, Point target)
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

        internal double Distance(Point a, Point b) => Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        private bool ccw(Point A, Point B, Point C) => (C.Y - A.Y) * (B.X - A.X) > (B.Y - A.Y) * (C.X - A.X);
        internal bool Intersect(Point A, Point B, Point C, Point D) => ccw(A, C, D) != ccw(B, C, D) && ccw(A, B, C) != ccw(A, B, D);
    }
}
