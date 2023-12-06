namespace Advent.Assignments
{
    internal class Day06_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var tTotal = input[0].Split(':')[1].AsSpan().ToLongIgnoreWhitespace();
            var distance = input[1].Split(':')[1].AsSpan().ToLongIgnoreWhitespace();
            var range = Solve(tTotal, distance);
            var wholeMin = (int)Math.Ceiling(range.Min);
            if (wholeMin == range.Min)
                wholeMin++;
            var wholeMax = (int)Math.Floor(range.Max);
            if (wholeMax == range.Max)
                wholeMax--;

            var options = wholeMax - wholeMin + 1;
            return options.ToString();
        }

        private (double Min, double Max) Solve(double tTotal, double distance)
        {
            checked
            {
                var sqrt = Math.Sqrt(tTotal * tTotal - 4 * distance);
                var a = (-tTotal - sqrt) / -2;
                var b = (-tTotal + sqrt) / -2;
                if (a < b)
                    return (a, b);
                return (b, a);
            }
        }
    }
}
