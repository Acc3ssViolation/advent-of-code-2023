using Advent.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day18_1 : IAssignment
    {
        private record Instruction(Direction Direction, int Distance, string Color)
        {
            public static Instruction Parse(string str)
            {
                var parts = str.Split(' ');
                var direction = parts[0][0] switch
                {
                    'U' => Direction.North,
                    'D' => Direction.South,
                    'L' => Direction.West,
                    'R' => Direction.East,
                    _ => throw new NotSupportedException()
                };
                var distance = parts[1].ToInt();
                var color = parts[2];
                return new Instruction(direction, distance, color);
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var position = Point.Zero;
            var grid = new CharGrid(500, 500);
            position += new Point(grid.Width / 2, grid.Height / 2);
            grid.Chars.Populate('.');
            foreach (var line in input)
            {
                var instruction = Instruction.Parse(line);
                var delta = instruction.Direction.ToVector();
                var goal = position + (delta * instruction.Distance);
                while (position != goal)
                {
                    //Debug.Assert(position.x >= 0 && position.y >= 0 && position.x < grid.Width && position.y < grid.Height);
                    grid[position] = '#';
                    position += delta;
                }
            }

            var outsideCount = FloodFill(grid, '.', Point.Zero);
            var insideCount = (grid.Width * grid.Height) - outsideCount;
            return insideCount.ToString();
        }

        private static int FloodFill(CharGrid grid, char free, Point start)
        {
            void TryAddPoint(Point point, Queue<Point> queue, int width, int height)
            {
                if (point.x < 0 || point.y < 0 || point.x >= width || point.y >= height)
                    return;
                queue.Enqueue(point);
            }

            var queue = new Queue<Point>();
            queue.Enqueue(start);
            var painted = 0;
            while (queue.TryDequeue(out var point))
            {
                var tile = grid[point];
                if (tile != free)
                    continue;

                grid[point] = '/';
                painted++;

                TryAddPoint(point + Point.North, queue, grid.Width, grid.Height);
                TryAddPoint(point + Point.East, queue, grid.Width, grid.Height);
                TryAddPoint(point + Point.South, queue, grid.Width, grid.Height);
                TryAddPoint(point + Point.West, queue, grid.Width, grid.Height);
            }
            return painted;
        }
    }
}
