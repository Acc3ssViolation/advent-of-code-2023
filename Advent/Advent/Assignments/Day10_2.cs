//using Advent.Shared;
//using System.Diagnostics;

//namespace Advent.Assignments
//{
//    internal partial class Day10_2 : IAssignment
//    {
//        //public string InputFile => "day10_alt.txt";
//        string IAssignment.TestFile => "test-day10_2.txt";

//        private const char WallLoop = 'W';
//        private const char Wall = 'X';
//        private const char Air = '.';

//        private static List<Point> GetPipePoints(IReadOnlyList<string> input)
//        {
//            var grid = new CharGrid(input);
//            var start = grid.Find('S');
//            var (place, next) =
//                FindStartNeighbour(start, Point.North, grid) ??
//                FindStartNeighbour(start, Point.East, grid) ??
//                FindStartNeighbour(start, Point.South, grid) ??
//                FindStartNeighbour(start, Point.West, grid) ?? throw new Exception();

//            var points = new List<Point>();
//            points.Add(start);
//            var count = 1;
//            while (true)
//            {
//                //Logger.DebugLine($"At {place}, moving to {next}");
//                count++;
//                points.Add(place);
//                (place, next) = GetNext(place, next, grid);
//                if (place == start)
//                    break;
//            }

//            return points;
//        }

//        private static CharGrid DrawPipe(IReadOnlyList<string> input)
//        {
//            var grid = new CharGrid(input);
//            var grid2 = new CharGrid(grid.Width, grid.Height);
//            grid2.Chars.Populate(Air);
//            var start = grid.Find('S');
//            grid2[start] = 'S';
//            var (place, next) =
//                FindStartNeighbour(start, Point.North, grid) ??
//                FindStartNeighbour(start, Point.East, grid) ??
//                FindStartNeighbour(start, Point.South, grid) ??
//                FindStartNeighbour(start, Point.West, grid) ?? throw new Exception();

//            var count = 1;
//            while (true)
//            {
//                //Logger.DebugLine($"At {place}, moving to {next}");
//                count++;
//                grid2[place] = grid[place];
//                (place, next) = GetNext(place, next, grid);
//                if (place == start)
//                    break;
//            }

//            return grid2;
//        }

//        private static CharGrid Compress(CharGrid expanded)
//        {
//            var grid = new CharGrid(expanded.Width / 3, expanded.Height / 3);
//            for (var y = 0; y < grid.Height; y++)
//            {
//                for (var x = 0; x < grid.Width; x++)
//                {
//                    var chr = expanded[new Point(x * 3 + 1, y * 3 + 1)];
//                    grid[new Point(x, y)] = chr;
//                }
//            }
//            return grid;
//        }

//        public string Run(IReadOnlyList<string> input)
//        {
//            FixStart
//            var grid = new CharGrid(input);
//            var start = Point.Zero;
//            var blitGrid = new CharGrid(grid.Width * 3, grid.Height * 3);
//            blitGrid.Chars.Populate(Air);
//            for (var y = 0; y < grid.Height; y++)
//            {
//                for (var x = 0; x < grid.Width; x++)
//                {
//                    var p = new Point(x, y);
//                    var chr = grid[p];
//                    if (chr == 'S')
//                        start = p;
//                    BlitSegment(blitGrid, p, chr);
//                }
//            }
//            FixStart(blitGrid, start);


//            //Logger.DebugLine(grid.ToString());
//            //Logger.DebugLine(blitGrid.ToString());
//            var pipePoints = GetPipePoints(input);
//            var solve = Solve(grid, pipePoints);
//            var str = blitGrid.ToString();
//            //Logger.DebugLine(str);

//            var pipes = DrawPipe(input);
//            var small = Compress(blitGrid);

//            Logger.DebugLine("============================");
//            Logger.DebugLine(pipes.ToString());
//            Logger.DebugLine("============================");
//            Logger.DebugLine(small.ToString());
//            Logger.DebugLine("============================");

//            return solve.ToString();
//        }

//        private static int Solve(CharGrid grid, List<Point> pipePoints)
//        {
//            var insideCount = 0;

//            for (var y = 0; y < grid.Height; y++)
//            {
//                var inside = false;
//                var lastCorner = 'F';
//                var points = new List<char>();
//                for (var x = 0; x < grid.Width; x++)
//                {
//                    var point = new Point(x, y);
//                    if (pipePoints.Contains(point))
//                    {
//                        switch (grid[point])
//                        {
//                            case 'F':
//                            case 'L':
//                                lastCorner = grid[point];
//                                break;
//                            case 'J':
//                                {
//                                    if (lastCorner == 'F')
//                                        inside = !inside;
//                                }
//                                break;
//                            case '7':
//                                {
//                                    if (lastCorner == 'L')
//                                        inside = !inside;
//                                }
//                                break;
//                            case '|':
//                                inside = !inside;
//                                break;
//                        }
//                        points.Add(grid[point]);
//                    }
//                    else if (grid[point] == Air)
//                    {
//                        if (inside)
//                        {
//                            grid[point] = 'I';
//                            insideCount++;
//                        }
//                        points.Add(' ');
//                    }
//                    else
//                    {
//                        points.Add(' ');
//                    }
//                }
//                Logger.DebugLine($"[{y:D2}] {string.Join(' ', points)}");
//            }

//            return insideCount;
//        }

//        private static int Solve(CharGrid grid, Point start)
//        {
//            var empty = 0;
//            //for (var x = 0; x <= grid.Width; x++)
//            //{
//            //    FloodFill(grid, new Point(x, 0), '0', ref empty);
//            //    FloodFill(grid, new Point(x, grid.Height - 1), '0', ref empty);
//            //}
//            //for (var y = 0; y <= grid.Height; y++)
//            //{
//            //    FloodFill(grid, new Point(0, y), '0', ref empty);
//            //    FloodFill(grid, new Point(grid.Width - 1, y), '0', ref empty);
//            //}

//            int count;
//            count = 0;
//            if (FloodFill(grid, start * 3 + new Point(2, 0), '#', ref count) == FillResult.Ok)
//                return count;

//            count = 0;
//            if (FloodFill(grid, start * 3 + new Point(0, 0), '/', ref count) == FillResult.Ok)
//                return count;

//            count = 0;
//            if (FloodFill(grid, start * 3 + new Point(2, 2), '\\', ref count) == FillResult.Ok)
//                return count;

//            count = 0;
//            if (FloodFill(grid, start * 3 + new Point(0, 2), '+', ref count) == FillResult.Ok)
//                return count;

//            return 0;
//        }


//        enum FillResult
//        {
//            FoundExit,
//            InWall,
//            FoundPrevious,
//            Ok,
//        }

//        private static FillResult FloodFill(CharGrid grid, Point startPoint, char fill, ref int count)
//        {
//            var queue = new Queue<Point>();
//            queue.Enqueue(startPoint);
//            while (queue.TryDequeue(out var point))
//            {
//                if (point.x < 0 || point.y < 0 || point.y >= grid.Height || point.x >= grid.Width)
//                    return FillResult.FoundExit;

//                var gridContent = grid[point];
//                // We're in a wall, nothing to do
//                if (gridContent == Wall || gridContent == 'S' || gridContent == fill || gridContent == '@')
//                    continue;
//                // Found a previous set of paths
//                //if (gridContent != Air)
//                //    return FillResult.FoundPrevious;

//                grid[point] = fill;
//                if (point.x % 3 == 1 && point.y % 3 == 1)
//                {
//                    count++;
//                    grid[point] = '@';
//                }

//                //Logger.DebugLine("=========================================================");
//                //Logger.DebugLine(grid.ToString());

//                FillResult TryEnqueuePoint(Point point, Queue<Point> queue)
//                {
//                    if (point.x < 0 || point.y < 0 || point.y >= grid.Height || point.x >= grid.Width)
//                        return FillResult.FoundExit;
//                    if (grid[point] != fill)
//                        queue.Enqueue(point);
//                    return FillResult.Ok;
//                }

//                if (TryEnqueuePoint(point + Point.East, queue) == FillResult.FoundExit)
//                    return FillResult.FoundExit;
//                if (TryEnqueuePoint(point + Point.West, queue) == FillResult.FoundExit)
//                    return FillResult.FoundExit;
//                if (TryEnqueuePoint(point + Point.North, queue) == FillResult.FoundExit)
//                    return FillResult.FoundExit;
//                if (TryEnqueuePoint(point + Point.South, queue) == FillResult.FoundExit)
//                    return FillResult.FoundExit;
//            }

//            return FillResult.Ok;
//        }

//        private static void FixStart(CharGrid grid, Point point)
//        {
//            var center = point * 3 + new Point(1, 1);

//            var north = center + Point.North * 2;
//            if (IsWall(grid, north))
//                grid[center + Point.North] = Wall;

//            var west = center + Point.West * 2;
//            if (IsWall(grid, west))
//                grid[center + Point.West] = Wall;

//            var east = center + Point.East * 2;
//            if (IsWall(grid, east))
//                grid[center + Point.East] = Wall;

//            var south = center + Point.South * 2;
//            if (IsWall(grid, south))
//                grid[center + Point.South] = Wall;
//        }

//        [Flags]
//        enum DirectionFlags
//        {
//            North = 1,
//            East = 2,
//            South = 4,
//            West = 8,
//        }

//        private static void FixStartInput(CharGrid grid, Point point)
//        {
//            var directions = DirectionFlags.
//            var north = point + Point.North * 2;
//            if (IsWall(grid, north))
//                grid[center + Point.North] = Wall;

//            var west = point + Point.West * 2;
//            if (IsWall(grid, west))
//                grid[center + Point.West] = Wall;

//            var east = point + Point.East * 2;
//            if (IsWall(grid, east))
//                grid[center + Point.East] = Wall;

//            var south = point + Point.South * 2;
//            if (IsWall(grid, south))
//                grid[center + Point.South] = Wall;
//        }

//        private static bool IsWall(CharGrid grid, Point point)
//        {
//            if (point.x < 0 || point.y < 0 || point.y >= grid.Height || point.x >= grid.Width)
//                return false;

//            return grid[point] == Wall;
//        }

//        private static void BlitSegment(CharGrid grid, Point point, char segment)
//        {
//            var center = point * 3 + new Point(1, 1);
//            switch (segment)
//            {
//                case '-':
//                    {
//                        grid[center + Point.West] = Wall;
//                        grid[center] = Wall;
//                        grid[center + Point.East] = Wall;
//                        break;
//                    }
//                case '|':
//                    {
//                        grid[center + Point.North] = Wall;
//                        grid[center] = Wall;
//                        grid[center + Point.South] = Wall;
//                        break;
//                    }
//                case 'L':
//                    {
//                        grid[center + Point.North] = Wall;
//                        grid[center] = Wall;
//                        grid[center + Point.East] = Wall;
//                        break;
//                    }
//                case 'J':
//                    {
//                        grid[center + Point.North] = Wall;
//                        grid[center] = Wall;
//                        grid[center + Point.West] = Wall;
//                        break;
//                    }
//                case 'F':
//                    {
//                        grid[center + Point.East] = Wall;
//                        grid[center] = Wall;
//                        grid[center + Point.South] = Wall;
//                        break;
//                    }
//                case '7':
//                    {
//                        grid[center + Point.West] = Wall;
//                        grid[center] = Wall;
//                        grid[center + Point.South] = Wall;
//                        break;
//                    }
//                case 'S':
//                    {
//                        grid[center] = 'S';
//                        break;
//                    }
//                default:
//                    break;
//            }
//        }



//        private static (Point Point, Point Next) GetNext(Point current, Point next, CharGrid grid)
//        {
//            var connections = GetConnections(grid[next], next);
//            if (connections.A == current)
//            {
//                return (next, connections.B);
//            }
//            else
//            {
//                return (next, connections.A);
//            }
//        }

//        private static (Point Point, Point Next)? FindStartNeighbour(Point start, Point dir, CharGrid grid)
//        {
//            var neighbour = start + dir;
//            if (neighbour.y >= 0 && neighbour.y < grid.Height && neighbour.x >= 0 && neighbour.x < grid.Width)
//            {
//                var connections = GetConnections(grid[neighbour], neighbour);
//                if (connections.A == start)
//                {
//                    return (neighbour, connections.B);
//                }
//                else if (connections.B == start)
//                {
//                    return (neighbour, connections.A);
//                }
//            }
//            return null;
//        }

//        private static (Point A, Point B) GetConnections(char segment, Point offset)
//        {
//            var (a, b) = GetConnections(segment);
//            return (a + offset, b + offset);
//        }

//        private static (Point A, Point B) GetConnections(char segment)
//        {
//            switch (segment)
//            {
//                case '-':
//                    return (Point.East, Point.West);
//                case '|':
//                    return (Point.North, Point.South);
//                case 'L':
//                    return (Point.North, Point.East);
//                case 'J':
//                    return (Point.North, Point.West);
//                case 'F':
//                    return (Point.South, Point.East);
//                case '7':
//                    return (Point.West, Point.South);
//                default:
//                    return (Point.Zero, Point.Zero);
//            }
//        }
//    }
//}
