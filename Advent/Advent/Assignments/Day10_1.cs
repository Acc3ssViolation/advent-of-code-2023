using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day10_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var grid = new StringGrid(input);
            var start = grid.Find('S');
            var (place, next) = 
                FindStartNeighbour(start, Point.North, grid) ??
                FindStartNeighbour(start, Point.East, grid) ??
                FindStartNeighbour(start, Point.South, grid) ??
                FindStartNeighbour(start, Point.West, grid) ?? throw new Exception();

            var count = 1;
            while (true)
            {
                //Logger.DebugLine($"At {place}, moving to {next}");
                count++;
                (place, next) = GetNext(place, next, grid);
                if (place == start)
                    break;
            }

            return (count / 2).ToString();
        }

        private static (Point Point, Point Next) GetNext(Point current, Point next, StringGrid grid)
        {
            var connections = GetConnections(grid[next], next);
            if (connections.A == current)
            {
                return (next, connections.B);
            }
            else
            {
                return (next, connections.A);
            }
        }

        private static (Point Point, Point Next)? FindStartNeighbour(Point start, Point dir, StringGrid grid)
        {
            var neighbour = start + dir;
            if (neighbour.y >= 0 && neighbour.y < grid.Height && neighbour.x >= 0 && neighbour.x < grid.Width)
            {
                var connections = GetConnections(grid[neighbour], neighbour);
                if (connections.A == start)
                {
                    return (neighbour, connections.B);
                }
                else if (connections.B == start)
                {
                    return (neighbour, connections.A);
                }
            }
            return null;
        }

        private static (Point A, Point B) GetConnections(char segment, Point offset)
        {
            var (a, b) = GetConnections(segment);
            return (a + offset, b + offset);
        }

        private static (Point A, Point B) GetConnections(char segment)
        {
            switch (segment)
            {
                case '-':
                    return (Point.East, Point.West);
                case '|':
                    return (Point.North, Point.South);
                case 'L':
                    return (Point.North, Point.East);
                case 'J':
                    return (Point.North, Point.West);
                case 'F':
                    return (Point.South, Point.East);
                case '7':
                    return (Point.West, Point.South);
                default:
                    return (Point.Zero, Point.Zero);
            }
        }

        class StringGrid
        {
            public int Width { get; }
            public int Height { get; }

            public char[] Chars => _data;

            private readonly char[] _data;

            public char this[Point point] => _data[point.x + point.y * Width];

            public StringGrid(IReadOnlyList<string> strings)
            {
                Width = strings[0].Length;
                Height = strings.Count;
                _data = new char[Width * Height];
                for (var i = 0; i < strings.Count; i++)
                {
                    strings[i].CopyTo(0, _data, i * Width, Width);
                }
            }

            public Point Find(char chr)
            {
                for (var y = 0; y < Height; y++)
                    for (var x = 0; x < Width; x++)
                    {
                        if (_data[y * Width + x] == chr)
                            return new Point(x, y);
                    }
                return default;
            }
        }
    }
}
