﻿using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day18_1 : IAssignment
    {
        private record Instruction(Point Direction, int Distance)
        {
            public static Instruction Parse(string str)
            {
                var parts = str.Split(' ');
                var direction = parts[0][0] switch
                {
                    'U' => Point.North,
                    'D' => Point.South,
                    'L' => Point.West,
                    'R' => Point.East,
                    _ => throw new NotSupportedException()
                };
                var distance = parts[1].ToInt();
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
                var direction = instruction.Direction.ToDirection();
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
