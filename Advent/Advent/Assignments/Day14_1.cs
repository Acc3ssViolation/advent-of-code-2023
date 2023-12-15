using Advent.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day14_1 : IAssignment
    {
        const char Solid = '#';
        const char Rock = 'O';
        const char Air = '.';

        public string Run(IReadOnlyList<string> input)
        {
            var grid = new CharGrid(input);

            //Logger.DebugLine(grid.ToString());

            var rocks = SlideRocks(grid);

            //Logger.DebugLine(grid.ToString());

            return rocks.ToString();
        }

        enum State
        {
            SearchingRock,

        }

        private static int SlideRocks(CharGrid grid)
        {
            var score = 0;

            //Logger.DebugLine(grid.ToString());

            for (var x = 0; x < grid.Width; x++)
            {
                var emptyStart = 0;         // Start of empty space
                var rocksStart = 0;         // Start of the rocks
                var rocksEnd = 0;           // 1 past the last rock
                var inRock = false;

                for (var y = 0; y < grid.Height; y++)
                {
                    var point = new Point(x, y);
                    var tile = grid[point];

                    if (inRock)
                    {
                        rocksEnd = y;
                        if (tile != Rock)
                        {
                            // Ran out of rocks to move
                            var distance = rocksStart - emptyStart;
                            // Move the entire block down
                            for (var ny = rocksStart; ny < rocksEnd; ny++)
                            {
                                grid[new Point(x, ny)] = Air;
                            }
                            for (var ny = rocksStart - distance; ny < rocksEnd - distance; ny++)
                            {
                                grid[new Point(x, ny)] = Rock;
                                var weight = grid.Height - ny;
                                score += weight;
                            }
                            // Start of next empty space is after the moved rocks
                            if (tile == Solid)
                            {
                                emptyStart = y + 1;
                            }
                            else
                            {
                                emptyStart = rocksEnd - distance;
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
                    rocksEnd = grid.Height;
                    // Ran out of rocks to move
                    var distance = rocksStart - emptyStart;
                    // Move the entire block down
                    for (var ny = rocksStart; ny < rocksEnd; ny++)
                    {
                        grid[new Point(x, ny)] = Air;
                    }
                    for (var ny = rocksStart - distance; ny < rocksEnd - distance; ny++)
                    {
                        grid[new Point(x, ny)] = Rock;
                        var weight = grid.Height - ny;
                        score += weight;
                    }
                }
            }

            //Logger.DebugLine(grid.ToString());

            return score;
        }
    }
}
