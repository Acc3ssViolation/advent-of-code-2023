using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day16_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var grid = new CharGrid(input);
            var lightPoints = new HashSet<Point>();
            
            Trace(Point.Zero, Point.East, grid, lightPoints);

            var power = lightPoints.Count;
            return power.ToString();
        }

        private static bool IsProcessed(char tile, Point dir)
        {
            var direction = dir.ToDirection();
            var flag = 1 << ((int)direction + 8);
            return (tile & flag) != 0;
        }

        private static char FlagProcessed(char tile, Point dir)
        {
            var direction = dir.ToDirection();
            var flag = 1 << ((int)direction + 8);
            return (char)(tile | flag);
        }

        private static void Trace(Point point, Point dir, CharGrid grid, HashSet<Point> lightPoints)
        {
            while (true)
            {
                if (point.x < 0 || point.y < 0 || point.x >= grid.Width || point.y >= grid.Height)
                    return;
                var tile = grid[point];
                if (IsProcessed(tile, dir))
                    return;
                grid[point] = FlagProcessed(tile, dir);
                lightPoints.Add(point);
                switch (tile & 0xFF)
                {
                    case '\\':
                    {
                        var tmp = dir.x;
                        dir.x = dir.y;
                        dir.y = tmp;
                    }
                    break;
                    case '/':
                    {
                        var tmp = dir.x;
                        dir.x = -dir.y;
                        dir.y = -tmp;
                    }
                    break;
                    case '-':
                    {
                        if (dir.y != 0)
                        {
                            // Split east and west
                            // We go east, recursion goes west
                            dir = Point.East;
                            Trace(point + Point.West, Point.West, grid, lightPoints);
                        }
                    }
                    break;
                    case '|':
                    {
                        if (dir.x != 0)
                        {
                            // Split north and south
                            // We go north, recursion goes south
                            dir = Point.North;
                            Trace(point + Point.South, Point.South, grid, lightPoints);
                        }
                    }
                    break;
                }

                // Move on to the next spot
                point += dir;
            }
        }
    }
}