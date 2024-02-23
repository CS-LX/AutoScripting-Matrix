using Engine;
using Engine.Graphics;

namespace Game {
    public static class ASMELEDUtils {

        public static (Point2, Point2) LargestRectangle(Point2[] points)
        {
            Array.Sort(points, (p1, p2) =>
            {
                if (p1.X == p2.X)
                {
                    return p1.Y.CompareTo(p2.Y);
                }
                return p1.X.CompareTo(p2.X);
            });

            int maxArea = 0;
            (Point2, Point2) maxRect = (new Point2(0, 0), new Point2(0, 0));

            for (int i = 0; i < points.Length; i++)
            {
                HashSet<int> xDiffs = new HashSet<int>();
                for (int j = i + 1; j < points.Length; j++)
                {
                    int xDiff = points[j].X - points[i].X;
                    xDiffs.Add(xDiff);
                }

                foreach (int xDiff in xDiffs)
                {
                    if (xDiffs.Count == 1)
                    {
                        continue;
                    }
                    int height = xDiffs.Count - xDiffs.Count(x => x == xDiff);
                    int width = xDiff;
                    int area = height * width;

                    if (area > maxArea)
                    {
                        maxArea = area;
                        maxRect = (new Point2(points[i].X, points[i].X), new Point2(points[i].X + width, points[i].X + height));
                    }
                }
            }

            return maxRect;
        }
    }
}