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

        private static int SlideRocks(CharGrid grid)
        {
            var score = 0;

            for (var y = 0; y < grid.Height; y++)
            {
                for (var x = 0; x < grid.Width; x++)
                {
                    var point = new Point(x, y);
                    var tile = grid[point];
                    if (tile != Rock)
                        continue;

                    var newPoint = new Point(x, y);
                    for (var ny = y - 1; ny >= 0; ny--)
                    {
                        var p = new Point(x, ny);
                        if (grid[p] != Air)
                            break;
                        newPoint = p;
                    }

                    if (newPoint.y != point.y)
                    {
                        grid[newPoint] = Rock;
                        grid[point] = Air;
                    }

                    var weight = grid.Height - newPoint.y;
                    score += weight;
                }
            }

            return score;
        }
    }
}
