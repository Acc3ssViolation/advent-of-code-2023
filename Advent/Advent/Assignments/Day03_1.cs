using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day03_1 : IAssignment
    {
        struct Point
        {
            public int x;
            public int y;

            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public override string ToString()
            {
                return $"[{x}, {y}]";
            }
        }

        class Num
        {
            public int Value { get; }
            public int StartX { get; }

            public Num(int value, int startX)
            {
                Value = value;
                StartX = startX;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var builder = new IntBuilder();
            var nums = new Dictionary<Point, Num>();
            var symbols = new List<(Point Point, char Symbol)>();
            var y = 0;
            foreach (var line in input)
            {
                builder.Clear();
                for (var x = 0; x < line.Length; x++)
                {
                    var isNum = builder.PushChar(line[x]);
                    if (isNum)
                        continue;

                    var digits = builder.Length;
                    if (digits > 0)
                    {
                        var num = builder.ToInt();
                        builder.Clear();
                        var box = new Num(num, x - digits);
                        for (var dx = 0; dx < digits; dx++)
                        {
                            nums.Add(new Point(x - 1 - dx, y), box);
                        }
                    }

                    if (line[x] == '.')
                        continue;

                    symbols.Add((new Point(x, y), line[x]));
                }
                {
                    var digits = builder.Length;
                    if (digits > 0)
                    {
                        var num = builder.ToInt();
                        builder.Clear();
                        var box = new Num(num, line.Length - digits);
                        for (var dx = 0; dx < digits; dx++)
                        {
                            nums.Add(new Point(line.Length - 1 - dx, y), box);
                        }
                    }
                }
                y++;
            }

            //var str = Print(nums, symbols, input[0].Length, input.Count);
            //Logger.Line(str);

            var sum = 0L;
            var coveredNums = new HashSet<Num>();
            foreach (var kv in symbols)
            {
                var symbol = kv.Point;
                void GetNumber(int x, int y)
                {
                    if (nums!.TryGetValue(new Point(x, y), out var num) && !coveredNums!.Contains(num))
                    {
                        //Logger.DebugLine($"{num} is adjacent to point {new Point(symbol.x, symbol.y)}");
                        coveredNums!.Add(num);
                        sum += num.Value;
                    }
                }

                GetNumber(symbol.x - 1, symbol.y - 1);
                GetNumber(symbol.x, symbol.y - 1);
                GetNumber(symbol.x + 1, symbol.y - 1);

                GetNumber(symbol.x - 1, symbol.y);

                GetNumber(symbol.x + 1, symbol.y);

                GetNumber(symbol.x - 1, symbol.y + 1);
                GetNumber(symbol.x, symbol.y + 1);
                GetNumber(symbol.x + 1, symbol.y + 1);
            }

            return sum.ToString();
        }

        private static string Print(Dictionary<Point, Num> nums, List<(Point Point, char Symbol)> points, int width, int height)
        {
            var sb = new StringBuilder();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    nums.TryGetValue(new Point(x, y), out var num);
                    var point = points.FirstOrDefault(p => p.Point.x == x && p.Point.y == y);

                    if (num != null)
                    {
                        var digit = num.Value.ToString()[x - num.StartX];
                        sb.Append(digit);
                    }
                    else if (point.Symbol != default(char))
                    {
                        sb.Append(point.Symbol);
                    }
                    else
                    {
                        sb.Append('.');
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
