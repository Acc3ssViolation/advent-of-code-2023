using Advent.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day14_2 : IAssignment
    {
        const char Solid = '#';
        const char Rock = 'O';
        const char Air = '.';

        public string Run(IReadOnlyList<string> input)
        {
            var grid = new CharGrid(input);

            //Logger.DebugLine(grid.ToString());

            var loop = FindLoop(grid, out var rockLists);

            var totalIterations = 1000000000;

            var modIteration = (totalIterations - loop.Start) % loop.Duration;

            var rockPositions = rockLists[loop.Start + modIteration - 1];

            //for (var i = 0; i < rockLists.Count; i++)
            //{
            //    var rockScore = GetScore(rockLists[i], grid.Height);
            //    var plot = PlotRocks(rockLists[i], grid.Width, grid.Height);
            //    Logger.DebugLine(plot);
            //    var extraText = string.Empty;
            //    if (i == loop.Start)
            //        extraText = "<---------------- START";
            //    else if ((i - loop.Start) % loop.Duration == 0)
            //        extraText = "<---------------- END";
            //    Logger.DebugLine($"[{i:D2}] {rockScore} {extraText}");
            //    Logger.DebugLine("-----------------------------------------");
            //}

            //Logger.DebugLine(PlotRocks(rockPositions, grid.Width, grid.Height));

            var score = GetScore(rockPositions, grid.Height);
            //Logger.DebugLine(grid.ToString());

            return score.ToString();
        }

        private static string PlotRocks(List<Point> rocks, int width, int height)
        {
            var grid = new CharGrid(width, height);
            grid.Chars.Populate(Air);
            foreach (var rock in rocks)
            {
                grid[rock] = Rock;
            }
            return grid.ToString();
        }

        private static int GetScore(List<Point> rocks, int height)
        {
            var score = 0;
            for (var i = 0; i < rocks.Count; i++)
                score += height - rocks[i].y;
            return score;
        }

        private static (int Start, int Duration) FindLoop(CharGrid grid, out List<List<Point>> rockLists)
        {
            rockLists = new List<List<Point>>();
            while (true)
            {
                var rocks = SlideRocks(grid);
                //Logger.DebugLine(PlotRocks(rocks, grid.Width, grid.Height));
                var duplicateIndex = rockLists.FindIndex(l => l.SequenceEqual(rocks));
                if (duplicateIndex != -1)
                {
                    var cycleCount = rockLists.Count - duplicateIndex;
                    return (duplicateIndex,  cycleCount);
                }
                rockLists.Add(rocks);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool SlideRock(CharGrid grid, Point point, Point dir, int maxAmount, out Point newPoint)
        {
            newPoint = new Point(point);
            var tile = grid[point];
            if (tile != Rock)
                return false;

            newPoint = new Point(point);
            for (var i = 1; i <= maxAmount; i++)
            {
                var p = point + (dir * i);
                if (grid[p] != Air)
                    break;
                newPoint = p;
            }

            if (newPoint != point)
            {
                grid[newPoint] = Rock;
                grid[point] = Air;
            }

            return true;
        }

        private static List<Point> SlideRocks(CharGrid grid)
        {
            //Logger.DebugLine(grid.ToString());

            // Slide north
            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    SlideRock(grid, new Point(x, y), Point.North, y, out var _);
                }
            }

            //Logger.DebugLine(grid.ToString());

            // Slide west
            for (var x = 0; x < grid.Width; x++)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    SlideRock(grid, new Point(x, y), Point.West, x, out var _);
                }
            }

            //Logger.DebugLine(grid.ToString());


            // Slide south
            for (var y = grid.Height - 1; y >= 0; y--)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    SlideRock(grid, new Point(x, y), Point.South, grid.Height - y - 1, out var _);
                }
            }

            //Logger.DebugLine(grid.ToString());

            var rocks = new List<Point>();
            // Slide east
            for (var x = grid.Width - 1; x >= 0; x--)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    if (SlideRock(grid, new Point(x, y), Point.East, grid.Width - x - 1, out var rockPoint))
                        rocks.Add(rockPoint);
                }
            }

            //Logger.DebugLine(grid.ToString());
            return rocks;
        }
    }
}
