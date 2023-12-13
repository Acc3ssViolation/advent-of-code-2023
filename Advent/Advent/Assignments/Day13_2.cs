using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day13_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var lines = new List<string>();
            var score = 0;
            foreach (var line in input)
            {
                if (line.Length > 0)
                {
                    lines.Add(line);
                    continue;
                }

                score += Process(lines);
            }
            score += Process(lines);
            return score.ToString();
        }

        private static int Process(List<string> lines)
        {
            var grid = new CharGrid(lines);

            // Logger.DebugLine("===================");
            var yMirror = IsMirroredVertically(grid, out var ySmuged);
            var score = (yMirror + 1) * 100;
            // Logger.DebugLine($"Mirror Y: {yMirror} {ySmuged}");
            if (yMirror < 0)
            {
                var xMirror = IsMirroredHorizontally(grid, out var xSmuged);
                // Logger.DebugLine($"Mirror X: {xMirror} {xSmuged}");
                score = (xMirror + 1);
            }
            // Logger.DebugLine($"Score: {score}");
            // Logger.DebugLine("===================");
            lines.Clear();

            return score;
        }

        private static int IsMirroredHorizontally(CharGrid grid, out bool smuged)
        {
            smuged = false;
            for (var x = 0; x < grid.Width - 1; x++)
            {
                smuged = false;

                var row1 = grid.GetCol(x);
                var row2 = grid.GetCol(x + 1);
                if (!SomewhatEquals(row1, row2, ref smuged))
                {
                    continue;
                }
                // Logger.DebugLine($"[{x}] {row1} == [{x + 1}] {row2} ({smuged})");

                // Potential mirror point, verify it
                var checkCount = Math.Min(x, grid.Width - x - 2);
                var verified = true;
                for (var i = 0; i < checkCount; i++)
                {
                    var row3 = grid.GetCol(x - 1 - i);
                    var row4 = grid.GetCol(x + 2 + i);
                    if (!SomewhatEquals(row3, row4, ref smuged))
                    {
                        verified = false;
                        break;
                    }
                    // Logger.DebugLine($"[{x - 1 - i}] {row3} == [{x + 2 + i}] {row4} ({smuged})");
                }

                if (verified && smuged)
                    return x;
            }

            return -1;
        }

        private static int IsMirroredVertically(CharGrid grid, out bool smuged)
        {
            smuged = false;
            for (var y = 0; y < grid.Height - 1; y++)
            {
                smuged = false;

                var row1 = grid.GetRow(y);
                var row2 = grid.GetRow(y + 1);
                if (!SomewhatEquals(row1, row2, ref smuged))
                {
                    continue;
                }
                // Logger.DebugLine($"[{y}] {row1} == [{y + 1}] {row2} ({smuged})");

                // Potential mirror point, verify it
                var checkCount = Math.Min(y, grid.Height - y - 2);
                var verified = true;
                for (var i = 0; i < checkCount; i++)
                {
                    var row3 = grid.GetRow(y - 1 - i);
                    var row4 = grid.GetRow(y + 2 + i);
                    if (!SomewhatEquals(row3, row4, ref smuged))
                    {
                        verified = false;
                        break;
                    }
                    // Logger.DebugLine($"[{y - 1 - i}] {row3} == [{y + 2 + i}] {row4} ({smuged})");
                }

                if (verified && smuged)
                    return y;
            }

            return -1;
        }

        private static bool SomewhatEquals(Span<char> a, Span<char> b, ref bool smudged)
        {
            if (smudged || a.Length != b.Length)
                return a.SequenceEqual(b);

            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    if (smudged)
                        return false;
                    smudged = true;
                }
            }
            return true;
        }
    }
}
