using Advent.Shared;
using System.Diagnostics;

namespace Advent.Assignments
{
    internal partial class Day10_2 : IAssignment
    {
        public string TestFile => "test-day10_2.txt";

        public string Run(IReadOnlyList<string> input)
        {
            var grid = new CharGrid(input);
            var loop = Prepare(grid);

            var insideCount = Scan(grid, loop);

            //Logger.DebugLine(grid.ToString());

            return insideCount.ToString();
        }

        private static int Scan(CharGrid grid, List<Point> loop)
        {
            var insideCount = 0;

            //var debugGrid = new CharGrid(grid.Width, grid.Height);
            //debugGrid.Chars.Populate('.');

            for (var y = 0; y < grid.Height; y++)
            {
                var inside = false;
                var fromBelow = false;
                var inBorder = false;

                for (var x = 0; x < grid.Width; x++)
                {
                    var point = new Point(x, y);
                    var tile = grid[point];
                    if (!loop.Contains(point))
                    {
                        // Empty space, count it if we're inside the loop
                        if (inside)
                        {
                            grid[point] = 'I';
                            //debugGrid[point] = 'I';
                            insideCount++;
                        }
                        continue;
                    }

                    if (tile == '|')
                    {
                        // Always an edge crossing
                        inside = !inside;

                        //debugGrid[point] = 'T';
                    }
                    else if (tile == '-')
                    {
                        // Ignore
                        Debug.Assert(inBorder);
                    }
                    else if (tile == 'F')
                    {
                        // We came from below
                        fromBelow = true;
                        inBorder = true;
                    }
                    else if (tile == 'L')
                    {
                        // We came from above
                        fromBelow = false;
                        inBorder = true;
                    }
                    else if (tile == 'J')
                    {
                        // We exit to above
                        Debug.Assert(inBorder);
                        // If we entered from below then this is a crossing
                        if (fromBelow)
                        {
                            inside = !inside;
                            //debugGrid[point] = 'T';
                        }
                        inBorder = false;
                    }
                    else if (tile == '7')
                    {
                        // We exit to below
                        Debug.Assert(inBorder);
                        // If we entered from above then this is a crossing
                        if (!fromBelow)
                        {
                            inside = !inside;
                            //debugGrid[point] = 'T';
                        }
                        inBorder = false;
                    }
                    else
                    {
                        Debug.Fail("Invalid");
                    }
                }
            }

            //Logger.DebugLine(debugGrid.ToString());

            return insideCount;
        }

        private static List<Point> Prepare(CharGrid grid)
        {
            var loop = new List<Point>();
            var start = grid.Find('S');
            var (place, next) = ReplaceStart(grid, start) ?? throw new Exception();
            //Logger.DebugLine(grid.ToString());
            loop.Add(start);
            while (true)
            {
                loop.Add(place);
                (place, next) = GetNext(place, next, grid);
                if (place == start)
                    break;
            }
            return loop;
        }

        private static (Point Point, Point Next)? ReplaceStart(CharGrid grid, Point start)
        {
            var north = FindStartNeighbour(start, Point.North, grid);
            var east = FindStartNeighbour(start, Point.East, grid);
            var south = FindStartNeighbour(start, Point.South, grid);
            var west = FindStartNeighbour(start, Point.West, grid);

            if (north.HasValue)
            {
                if (west.HasValue)
                {
                    grid[start] = 'J';
                }
                else if (south.HasValue)
                {
                    grid[start] = '|';
                }
                else
                {
                    grid[start] = 'L';
                }
            }
            else if (south.HasValue)
            {
                if (west.HasValue)
                {
                    grid[start] = '7';
                }
                else
                {
                    grid[start] = 'F';
                }
            }
            else
            {
                grid[start] = '-';
            }

            return north ?? east ?? south ?? west;
        }

        private static (Point Point, Point Next) GetNext(Point current, Point next, CharGrid grid)
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

        private static (Point Point, Point Next)? FindStartNeighbour(Point start, Point dir, CharGrid grid)
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
    }
}
