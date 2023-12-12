using Advent.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day11_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var galaxies = new List<Point>();
            var xArray = new BitArray(input[0].Length);
            var yArray = new BitArray(input.Count);

            for (var y = 0; y < input.Count; y++)
            {
                var line = input[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        galaxies.Add(new Point(x, y));
                        xArray.Set(x, true);
                        yArray.Set(y, true);
                    }
                }
            }

            // Expand in Y direction
            var yShift = 0;
            for (var y = 0; y < yArray.Count; y++)
            {
                if (yArray[y])
                    continue;

                for (var i = 0; i < galaxies.Count; i++)
                {
                    if (galaxies[i].y >= (y + yShift))
                    {
                        var galaxy = galaxies[i];
                        galaxy.y++;
                        galaxies[i] = galaxy;
                    }
                }
                yShift++;
            }

            // Expand in X direction
            var xShift = 0;
            for (var x = 0; x < xArray.Count; x++)
            {
                if (xArray[x])
                    continue;

                for (var i = 0; i < galaxies.Count; i++)
                {
                    if (galaxies[i].x >= (x + xShift))
                    {
                        var galaxy = galaxies[i];
                        galaxy.x++;
                        galaxies[i] = galaxy;
                    }
                }
                xShift++;
            }

            // Pairing
            var distanceSum = 0;
            var pairLoopCount = 0;
            for (var i = 0; i < galaxies.Count; i++)
            {
                for (var j = i + 1; j <  galaxies.Count; j++)
                {
                    var a = galaxies[i];
                    var b = galaxies[j];

                    var distance = Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
                    distanceSum += distance;

                    pairLoopCount++;
                }
            }

            return distanceSum.ToString();
        }
    }
}
