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
            rockLists = new List<List<Point>>(200);
            var westGrid = new CharGridSwapXY(grid);
            var southGrid = new CharGridMirrorY(grid);
            var eastGrid = new CharGridSwapXYMirrorY(grid);
            while (true)
            {
                var rocks = SlideRocks(grid, westGrid, southGrid, eastGrid);
                //Logger.DebugLine(PlotRocks(rocks, grid.Width, grid.Height));
                var duplicateIndex = rockLists.FindIndex(l => l.SequenceEqual(rocks));
                if (duplicateIndex != -1)
                {
                    var cycleCount = rockLists.Count - duplicateIndex;
                    return (duplicateIndex, cycleCount);
                }
                rockLists.Add(rocks);
            }
        }

        private static List<Point> SlideRocks(ICharGrid northGrid, ICharGrid westGrid, ICharGrid southGrid, ICharGrid eastGrid)
        {
            SlideRocksNorth(northGrid);
            SlideRocksNorth(westGrid);
            SlideRocksNorth(southGrid);
            SlideRocksNorth(eastGrid);

            //Logger.DebugLine(grid.ToString());
            //Logger.DebugLine(grid.ToString());

            var rocks = new List<Point>(2000);
            // Slide east
            for (var y = 0; y < northGrid.Height; y++)
            {
                for (var x = 0; x < northGrid.Width; x++)
                {
                    var rockPoint = new Point(x, y);
                    if (northGrid[rockPoint] == Rock)
                        rocks.Add(rockPoint);
                }
            }

            //Logger.DebugLine(grid.ToString());
            return rocks;
        }

        private static void SlideRocksNorth(ICharGrid grid)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                var emptyStart = 0;         // Start of empty space
                var rocksStart = 0;         // Start of the rocks
                var inRock = false;

                for (var y = 0; y < grid.Height; y++)
                {
                    var point = new Point(x, y);
                    var tile = grid[point];

                    if (inRock)
                    {
                        if (tile != Rock)
                        {
                            // Ran out of rocks to move
                            var distance = rocksStart - emptyStart;
                            // Move the entire block down
                            for (var ny = rocksStart; ny < y; ny++)
                            {
                                grid[new Point(x, ny)] = Air;
                            }
                            for (var ny = rocksStart - distance; ny < y - distance; ny++)
                            {
                                grid[new Point(x, ny)] = Rock;
                            }
                            // Start of next empty space is after the moved rocks
                            if (tile == Solid)
                            {
                                emptyStart = y + 1;
                            }
                            else
                            {
                                emptyStart = y - distance;
                            }
                            inRock = false;
                        }
                    }
                    else
                    {
                        // Not yet in a rock
                        if (tile == Rock)
                        {
                            // Found the start of a rock formation
                            rocksStart = y;
                            inRock = true;
                        }
                        else if (tile == Solid)
                        {
                            // The next one might be empty
                            emptyStart = y + 1;
                        }
                    }
                }

                if (inRock)
                {
                    // Ran out of rocks to move
                    var distance = rocksStart - emptyStart;
                    // Move the entire block down
                    for (var ny = rocksStart; ny < grid.Height; ny++)
                    {
                        grid[new Point(x, ny)] = Air;
                    }
                    for (var ny = rocksStart - distance; ny < grid.Height - distance; ny++)
                    {
                        grid[new Point(x, ny)] = Rock;
                    }
                }
            }
        }
    }
}
