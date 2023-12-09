using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day06_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var row1 = input[0].ExtractInts();
            var row2 = input[1].ExtractInts();

            var totalOptions = 1;
            for (var i = 0; i < row1.Count; i++)
            {
                var tTotal = row1[i];
                var distance = row2[i];
                var range = Solve(tTotal, distance);
                var wholeMin = (int)Math.Ceiling(range.Min);
                if (wholeMin == range.Min)
                    wholeMin++;
                var wholeMax = (int)Math.Floor(range.Max);
                if (wholeMax == range.Max)
                    wholeMax--;

                var options = wholeMax - wholeMin + 1;

                //Logger.DebugLine($"{options} ways to beat Time {row1[i]} Distance {row2[i]} [{range}]");
                totalOptions *= options;
            }

            return totalOptions.ToString();
        }

        private (double Min, double Max) Solve(int tTotal, int distance)
        {
            var sqrt = Math.Sqrt(tTotal * tTotal - 4 * distance);
            var a = (-tTotal - sqrt) / -2;
            var b = (-tTotal + sqrt) / -2;
            if (a < b)
                return (a, b);
            return (b, a);
        }

        private (double Min, double Max) SolveAbc(int a, int b, int c)
        {
            var sqrt = Math.Sqrt(b * b - 4 * a * c);
            var d0 = (-b + sqrt) / (2 * a);
            var d1 = (-b - sqrt) / (2 * a);
            if (d0 < d1)
                return (d0, d1);
            return (d1, d0);
        }
    }
}
