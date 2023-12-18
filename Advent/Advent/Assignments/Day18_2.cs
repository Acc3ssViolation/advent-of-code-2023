using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day18_2 : IAssignment
    {
        private record Instruction(Point Direction, int Distance)
        {
            public static Instruction Parse(string str)
            {
                var parts = str.Split(' ');
                var hexValue = parts[2].Substring(2).ToHexInt();
                var direction = (hexValue & 0x3) switch
                {
                    0 => Point.East,
                    1 => Point.South,
                    2 => Point.West,
                    _ => Point.North,
                };
                var distance = hexValue >> 4;
                return new Instruction(direction, distance);
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var vertices = new List<Point>();
            var position = Point.Zero;

            foreach (var line in input)
            {
                var instruction = Instruction.Parse(line);
                var delta = instruction.Direction * instruction.Distance;
                var newPosition = position + delta;
                vertices.Add(newPosition);
                position = newPosition;
            }

            var insideCount = vertices.Expand(0.5).Shoelace();
            return insideCount.ToString();
        }
    }
}
