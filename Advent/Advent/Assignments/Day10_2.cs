using Advent.Shared;

namespace Advent.Assignments
{
    internal partial class Day10_2 : IAssignment
    {
        string IAssignment.TestFile => "test-day10_2.txt";

        private const char Wall = 'X';
        private const char Air = '.';

        public string Run(IReadOnlyList<string> input)
        {
            var grid = new CharGrid(input);
            var start = Point.Zero;
            var blitGrid = new CharGrid(grid.Width * 3, grid.Height * 3);
            blitGrid.Chars.Populate(Air);
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var p = new Point(x, y);
                    var chr = grid[p];
                    if (chr == 'S')
                        start = p;
                    BlitSegment(blitGrid, p, chr);
                }
            }
            FixStart(blitGrid, start);


            //Logger.DebugLine(grid.ToString());
            //Logger.DebugLine(blitGrid.ToString());

            var solve = Solve(blitGrid, start);
            var str = blitGrid.ToString();
            Logger.DebugLine(str);
            return solve.ToString();
        }

        private static int Solve(CharGrid grid, Point start)
        {
            var empty = 0;
            for (var x = 0; x <= grid.Width; x++)
            {
                FloodFill(grid, new Point(x, 0), '0', ref empty);
                FloodFill(grid, new Point(x, grid.Height - 1), '0', ref empty);
            }
            for (var y = 0; y <= grid.Height; y++)
            {
                FloodFill(grid, new Point(0, y), '0', ref empty);
                FloodFill(grid, new Point(grid.Width - 1, y), '0', ref empty);
            }

            int count;
            count = 0;
            if (!FloodFill(grid, start * 3 + new Point(2, 0), '#', ref count))
                return count;

            count = 0;
            if (!FloodFill(grid, start * 3 + new Point(0, 0), '/', ref count))
                return count;

            count = 0;
            if (!FloodFill(grid, start * 3 + new Point(2, 2), '\\', ref count))
                return count;

            count = 0;
            if (!FloodFill(grid, start * 3 + new Point(0, 2), '+', ref count))
                return count;

            return count;
        }

        private static bool FloodFill(CharGrid grid, Point point, char fill, ref int count)
        {
            if (point.x < 0 || point.y < 0 || point.y >= grid.Height || point.x >= grid.Width)
                return false;

            var gridContent = grid[point];
            if (gridContent == Wall)
                return false;
            else if (gridContent == fill)
                return false;
            else if (gridContent != Air)
                return true;

            grid[point] = fill;
            if (point.x % 3 == 1 && point.y % 3 == 1)
            {
                count++;
            }

            if (FloodFill(grid, point + Point.East, fill, ref count))
                return true;
            if (FloodFill(grid, point + Point.West, fill, ref count))
                return true;
            if (FloodFill(grid, point + Point.South, fill, ref count))
                return true;
            if (FloodFill(grid, point + Point.North, fill, ref count))
                return true;
            return false;
        }

        private static void FixStart(CharGrid grid, Point point)
        {
            var center = point * 3 + new Point(1, 1);

            var north = center + Point.North * 2;
            if (IsWall(grid, north))
                grid[center + Point.North] = Wall;

            var west = center + Point.West * 2;
            if (IsWall(grid, west))
                grid[center + Point.West] = Wall;

            var east = center + Point.East * 2;
            if (IsWall(grid, east))
                grid[center + Point.East] = Wall;

            var south = center + Point.South * 2;
            if (IsWall(grid, south))
                grid[center + Point.South] = Wall;
        }

        private static bool IsWall(CharGrid grid, Point point)
        {
            if (point.x < 0 || point.y < 0 || point.y >= grid.Height || point.x >= grid.Width)
                return false;

            return grid[point] == Wall;
        }

        private static void BlitSegment(CharGrid grid, Point point, char segment)
        {
            var center = point * 3 + new Point(1, 1);
            switch (segment)
            {
                case '-':
                    {
                        grid[center + Point.West] = Wall;
                        grid[center] = Wall;
                        grid[center + Point.East] = Wall;
                        break;
                    }
                case '|':
                    {
                        grid[center + Point.North] = Wall;
                        grid[center] = Wall;
                        grid[center + Point.South] = Wall;
                        break;
                    }
                case 'L':
                    {
                        grid[center + Point.North] = Wall;
                        grid[center] = Wall;
                        grid[center + Point.East] = Wall;
                        break;
                    }
                case 'J':
                    {
                        grid[center + Point.North] = Wall;
                        grid[center] = Wall;
                        grid[center + Point.West] = Wall;
                        break;
                    }
                case 'F':
                    {
                        grid[center + Point.East] = Wall;
                        grid[center] = Wall;
                        grid[center + Point.South] = Wall;
                        break;
                    }
                case '7':
                    {
                        grid[center + Point.West] = Wall;
                        grid[center] = Wall;
                        grid[center + Point.South] = Wall;
                        break;
                    }
                case 'S':
                    {
                        grid[center] = 'S';
                        break;
                    }
                default:
                    break;
            }
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
