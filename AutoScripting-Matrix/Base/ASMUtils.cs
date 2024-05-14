using Engine;
using Engine.Graphics;

namespace Game {
    public static class ASMUtils {

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

        public static void FaceToAxesAndConner(int face, out Point3[] axes, out Point3[] conners) {
            switch (face) {
                case 0:
                    axes = [Point3.UnitY, Point3.UnitX, -Point3.UnitY, -Point3.UnitX];
                    conners = [new Point3(-1, 1, 0), new Point3(1, 1, 0), new Point3(1, -1, 0), new Point3(-1, -1, 0)];
                    break;
                case 1:
                    axes = [Point3.UnitY, -Point3.UnitZ, -Point3.UnitY, Point3.UnitZ];
                    conners = [new Point3(0, 1, 1), new Point3(0, 1, -1), new Point3(0, -1, -1), new Point3(0, -1, 1)];
                    break;
                case 2:
                    axes = [Point3.UnitY, -Point3.UnitX, -Point3.UnitY, Point3.UnitX];
                    conners = [new Point3(1, 1, 0), new Point3(-1, 1, 0), new Point3(-1, -1, 0), new Point3(1, -1, 0)];
                    break;
                case 3:
                    axes = [Point3.UnitY, Point3.UnitZ, -Point3.UnitY, -Point3.UnitZ];
                    conners = [new Point3(0, 1, -1), new Point3(0, 1, 1), new Point3(0, -1, 1), new Point3(0, -1, -1)];
                    break;
                case 4:
                    axes = [-Point3.UnitZ, Point3.UnitX, Point3.UnitZ, -Point3.UnitX];
                    conners = [new Point3(-1, 0, -1), new Point3(1, 0, -1), new Point3(1, 0, 1), new Point3(-1, 0, 1)];
                    break;
                case 5:
                    axes = [Point3.UnitZ, Point3.UnitX, -Point3.UnitZ, -Point3.UnitX];
                    conners = [new Point3(-1, 0, 1), new Point3(1, 0, 1), new Point3(1, 0, -1), new Point3(-1, 0, -1)];
                    break;
                default:
                    axes = [-Point3.UnitZ, Point3.UnitX, Point3.UnitZ, -Point3.UnitX];
                    conners = [new Point3(-1, 0, -1), new Point3(1, 0, -1), new Point3(1, 0, 1), new Point3(-1, 0, 1)];
                    break;
            }
        }

        public static Point2[] Point3ToPoint2(Point3[] points, int face) {
            switch (face) {
                case 0:
                case 2: return points.Select(p => new Point2(p.X, p.Y)).ToArray();
                case 1:
                case 3: return points.Select(p => new Point2(p.Y, p.Z)).ToArray();
                default: return points.Select(p => new Point2(p.X, p.Z)).ToArray();
            }
        }

        public static Vector2[] Vector3ToVector2(Vector3[] points, int face) {
            switch (face) {
                case 0:
                case 2: return points.Select(p => new Vector2(p.X, p.Y)).ToArray();
                case 1:
                case 3: return points.Select(p => new Vector2(p.Y, p.Z)).ToArray();
                default: return points.Select(p => new Vector2(p.X, p.Z)).ToArray();
            }
        }

        public static Vector2 Vector3ToVector2(Vector3 p, int face) {
            switch (face) {
                case 0: return new Vector2(-p.X, -p.Y);
                case 2: return new Vector2(p.X, p.Y);
                case 1: return new Vector2(-p.Y, -p.Z);
                case 3: return new Vector2(p.Y, p.Z);
                case 4: return new Vector2(p.X, p.Z);
                default: return new Vector2(-p.X, -p.Z);
            }
        }

        public static bool CheckRectangle(Point2[] points, out Point2 min, out int w, out int h) {
            w = h = 0;
            min = points[0];
            Point2 max = points[0];
            for (int i = 0; i < points.Length; i++)
            {
                min.X = MathUtils.Min(min.X, points[i].X);
                min.Y = MathUtils.Min(min.Y, points[i].Y);

                max.X = MathUtils.Max(max.X, points[i].X);
                max.Y = MathUtils.Max(max.Y, points[i].Y);
            }

            int square = (max.X - min.X + 1) * (max.Y - min.Y + 1);
            if(points.Length != square) return false;

            foreach (Point2 p in points)
            {
                if (p.X < min.X ||  p.Y < min.Y) return false;
                if (p.X > max.X || p.Y > max.Y) return false;
            }
            w = max.X - min.X + 1;
            h = max.Y - min.Y + 1;
            return true;
        }

        public static int[] NormalToFaces(Vector3 normal, float threshold) {
            List<int> faces = new();
            for (int i = 0; i < 6; i++) {
                if (faces.Count >= 4) return faces.ToArray();
                Vector3 faceNormal = CellFace.FaceToVector3(i);
                if(MathUtils.Abs(Vector3.Dot(faceNormal, normal)) < threshold) faces.Add(i);
            }
            return faces.ToArray();
        }

        public static int GetOppositeFace(int face) {
            return face switch {
                0 => 2,
                1 => 3,
                2 => 0,
                3 => 1,
                4 => 5,
                5 => 4,
                _ => 0
            };
        }
    }
}