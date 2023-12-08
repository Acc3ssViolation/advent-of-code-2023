using System;
using System.Diagnostics.CodeAnalysis;

namespace Advent.Assignments
{
    internal class Day08_1 : IAssignment, IEqualityComparer<ReadOnlyMemory<char>>
    {
        public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
            => x.Span.SequenceEqual(y.Span);

        public int GetHashCode([DisallowNull] ReadOnlyMemory<char> obj)
        {
            var span = obj.Span;
            return span[0] + span[1] + span[2];
        }

        public string Run(IReadOnlyList<string> input)
        {
            var dict = new Dictionary<ReadOnlyMemory<char>, (ReadOnlyMemory<char> Left, ReadOnlyMemory<char> Right)>(this);

            var commands = input[0];
            ReadOnlyMemory<char> place = default;
            ReadOnlyMemory<char> end = default;

            foreach (var line in input.Skip(2))
            {
                var key = line.AsMemory(0, 3);
                var left = line.AsMemory(7, 3);
                var right = line.AsMemory(12, 3);

                if (place.IsEmpty && Equals(key, "AAA".AsMemory()))
                    place = key;
                else if (end.IsEmpty && Equals(key, "ZZZ".AsMemory()))
                    end = key;
                
                dict.Add(key, (left, right));
            }

            var steps = 0;

            var commandIndex = 0;
            while (!Equals(place, end))
            {
                var options = dict[place];
                var command = commands[commandIndex++];
                if (commandIndex >= commands.Length)
                    commandIndex = 0;
                place = command == 'L' ? options.Left : options.Right;
                steps++;
            }

            return steps.ToString();
        }
    }
}
