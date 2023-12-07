using Advent.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day03_2 : IAssignment
    {
        class Num
        {
            public int Value { get; }

            public Num(int value)
            {
                Value = value;
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
                        var box = new Num(num);
                        for (var dx = 0; dx < digits; dx++)
                        {
                            nums.Add(new Point(x - 1 - dx, y), box);
                        }
                    }

                    if (line[x] != '*')
                        continue;

                    symbols.Add((new Point(x, y), line[x]));
                }
                {
                    var digits = builder.Length;
                    if (digits > 0)
                    {
                        var num = builder.ToInt();
                        builder.Clear();
                        var box = new Num(num);
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
            foreach (var kv in symbols)
            {
                var symbol = kv.Point;
                var gearNums = new List<Num>(3);

                void GetNumber(int x, int y)
                {
                    if (nums!.TryGetValue(new Point(x, y), out var num))
                    {
                        if (gearNums.Count < 3 && !gearNums.Contains(num))
                            gearNums.Add(num);
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

                if (gearNums.Count != 2)
                    continue;

                sum += gearNums[0].Value * gearNums[1].Value;
            }

            return sum.ToString();
        }
    }
}
