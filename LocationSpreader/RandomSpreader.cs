using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocationSpreader
{
    public class RandomSpreader : BaseSpreader
    {
        /// <summary>
        /// Simply places points by random. Points cannot intersect!
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="count"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public override List<Point> GenerateSuitableLocations(int width, int height, int count, int radius)
        {
            Random rnd = new Random();
            List<Point> points = new List<Point>();
            points.Add(new Point(
                        rnd.Next(radius, width - radius),
                        rnd.Next(radius, height - radius)
                        ));
            for (int i = 0; i < count - 1; i++)
            {
                var newPoint = new Point(
                        rnd.Next(radius, width - radius),
                        rnd.Next(radius, height - radius)
                        );
                int current = 0;
                int limit = 100;
                while (GetClosestPointDistance(points, newPoint) < radius * 3)
                {
                    newPoint = new Point(
                        rnd.Next(radius, width - radius),
                        rnd.Next(radius, height - radius)
                        );
                    current++;
                    if (current > limit)
                        break;
                }

                points.Add(newPoint);
            }

            return points;
        }
    }
}
