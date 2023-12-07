namespace Advent.Shared
{
    internal struct Point
    {
        public int x;
        public int y;

        public Point()
        {
            x = 0;
            y = 0;
        }

        public Point(Point point)
        {
            x = point.x;
            y = point.y;
        }

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point operator +(Point a) => a;
        public static Point operator -(Point a) => new(-a.x, -a.y);
        public static Point operator +(Point a, Point b) => new(a.x + b.x, a.y + b.y);
        public static Point operator -(Point a, Point b) => new(a.x - b.x, a.y - b.y);
        public static Point operator *(int n, Point a) => new(a.x * n, a.y * n);
        public static Point operator *(Point a, int n) => new(a.x * n, a.y * n);

        public override string ToString()
        {
            return $"[{x},{y}]";
        }
    }
}
