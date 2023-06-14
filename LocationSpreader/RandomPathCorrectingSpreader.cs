using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocationSpreader
{
    public class RandomPathCorrectingSpreader : BaseSpreader
    {
        public int Retries { get; set; } = 1000;

        /// <summary>
        /// Attepts to place the points so the distance between the ordered set is the smallest, as well as no line intersection between them.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="count"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public override List<Point> GenerateSuitableLocations(int width, int height, int count, int radius)
        {
            Random rnd = new Random();
            ILocationSpreader rndSpreader = new RandomSpreader();
            var points = rndSpreader.GenerateSuitableLocations(width, height, count, radius);

            bool changed = true;
            int tries = 0;
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
                if (!changed)
                {
                    for (int i = 0; i < points.Count - 1; i++)
                    {
                        var a1 = points[i];
                        var a2 = points[i + 1];

                        for (int j = i + 2; j < points.Count - 1; j++)
                        {
                            var b1 = points[j];
                            var b2 = points[j + 1];

                            if (Intersect(a1, a2, b1, b2))
                            {
                                var rid1 = rnd.Next(0, points.Count);
                                while (rid1 == i && rid1 != i + 1)
                                    rid1 = rnd.Next(0, points.Count);

                                var rp1 = points[rid1];
                                points[rid1] = a1;
                                points[i] = rp1;

                                changed = true;
                                i = points.Count;
                                break;
                            }
                        }
                    }
                }
                tries++;
                if (tries > Retries)
                    break;
            }

            return points;
        }
    }
}
