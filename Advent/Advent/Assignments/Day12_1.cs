
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day12_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sum = 0L;
            foreach (var item in input)
            {
                var parts = item.Split(' ');
                var record = parts[0];
                var groups = parts[1].ExtractInts();
                var options = RecursionMem(record, groups);
                sum += options;
                //Logger.DebugLine($"{record} {string.Join(',', groups)} has {options} options");
            }
            return sum.ToString();
        }

        private class Comparer : IEqualityComparer<(string, List<int>)>
        {
            public bool Equals((string, List<int>) x, (string, List<int>) y)
            {
                if (!string.Equals(x.Item1, y.Item1, StringComparison.Ordinal))
                    return false;
                return x.Item2.SequenceEqual(y.Item2);
            }

            public int GetHashCode([DisallowNull] (string, List<int>) obj)
            {
                return HashCode.Combine(obj.Item1, obj.Item2.Count);
            }
        }

        public static readonly Func<string, List<int>, long> RecursionMem = Memoization.Memoized<string, List<int>, long>(SolveRecursive, new Comparer());
        
        public static long SolveRecursive(string input, List<int> groups)
        {
            if (input.Length == 0)
            {
                return groups.Count == 0 ? 1 : 0;
            }

            var chr = input[0];
            if (chr == '.')
            {
                return RecursionMem(input.Substring(1), groups);
            }
            else if (chr == '?')
            {
                var mutableInput = input.ToCharArray();
                mutableInput[0] = '#';
                var sum = RecursionMem(new string(mutableInput), groups);
                mutableInput[0] = '.';
                sum += RecursionMem(new string(mutableInput), groups);
                return sum;
            }
            else
            {
                Debug.Assert(chr == '#');

                if (groups.Count == 0)
                    return 0;

                var expectedCount = groups[0];
                if (input.Length < expectedCount)
                    return 0;

                for (var i = 1; i < expectedCount; i++)
                {
                    if (input[i] == '.')
                        return 0;
                }

                // Next one after a group MUST be '.' or end of the line
                if (input.Length == expectedCount)
                {
                    // Done, we're out of space and this is the last group
                    if (groups.Count == 1)
                        return 1;
                    // We have a group left and we are out of space, oops
                    return 0;
                }

                // Ensure next char is a '.'
                if (input[expectedCount] == '#')
                {
                    // The group that fits here must be longer, no match
                    return 0;
                }
                // Set the next char to a '.' and go
                var mutableInput = input.Substring(expectedCount).ToCharArray();
                mutableInput[0] = '.';
                return RecursionMem(new string(mutableInput), groups.Skip(1).ToList());
            }
        }

    }
}
