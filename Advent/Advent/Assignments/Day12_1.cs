
using System.Diagnostics;
using System.Text;

namespace Advent.Assignments
{
    internal class Day12_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            return string.Empty;

            var total = 0;
            foreach (var line in input)
            {
                var parts = line.Split(' ');
                var groups = parts[1].ExtractInts();
                var record = parts[0];
                var states = GeneratePossibleStates(record).ToList();
                var validCount = 0;
                foreach (var state in states)
                {
                    //Logger.DebugLine($"\t{record} => {state}");
                    if (Validate(state, groups))
                        validCount++;
                }
                total += validCount;
                //Logger.DebugLine($"{line} => {validCount}");
            }

            return total.ToString();
        }

        private static bool Validate(string state, List<int> groups)
        {
            var parts = state.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != groups.Count)
                return false;

            for (var i = 0; i < parts.Length; i++)
            {
                if (parts[i].Length != groups[i])
                    return false;
            }

            return true;
        }

        private static IEnumerable<string> GeneratePossibleStates(string input)
        {
            if (input.Length == 0)
            {
                yield return input;
                yield break;
            }

            if (input[0] == '?')
            {
                foreach (var result in GeneratePossibleStates(input.Substring(1)))
                {
                    yield return "." + result;
                    yield return "#" + result;
                }
            }
            else
            {
                foreach (var result in GeneratePossibleStates(input.Substring(1)))
                {
                    yield return input[0] + result;
                }
            }
        }

        private class SpringRecord
        {
            private class Group
            {
                public string Data { get; }
                public int BrokenSprings { get; }
                public int UnknownSprings { get; }

                public Group(string data)
                {
                    Data = data;
                    for (var i = 0; i < data.Length; i++)
                    {
                        var chr = data[i];
                        if (chr == '?')
                            UnknownSprings++;
                        else if (chr == '#')
                            BrokenSprings++;
                    }
                }

                public IReadOnlyList<Group> Apply(int brokenSprings)
                {
                    // This group already contains more springs, it cannot match
                    if (brokenSprings < BrokenSprings)
                        return Array.Empty<Group>();

                    // This group cannot support this many springs
                    var springsToAdd = brokenSprings - BrokenSprings;
                    if (springsToAdd > UnknownSprings)
                        return Array.Empty<Group>();

                    // This group is fully resolved
                    if (UnknownSprings == 0)
                        return new[] { this };

                    // We use all slots
                    if (springsToAdd == UnknownSprings)
                        return new[] { new Group(new string('#', Data.Length)) };

                    // We will have some unknowns remaining... See what we can do
                    var totalSprings = Data.Length;
                    var maxShift = totalSprings - brokenSprings;
                    var result = new List<Group>();
                    for (var i = 0; i < maxShift + 1; i++)
                    {
                        // Try all options?
                        var sb = new StringBuilder();
                        var combinedNewSprings = 0;
                        var newSpringsAdded = 0;
                        for (var k = 0; k < Data.Length; k++)
                        {
                            var addSpring = false;
                            if (Data[k] == '#')
                            {
                                addSpring = true;
                            }

                            if (k >= i && newSpringsAdded < brokenSprings)
                            {
                                newSpringsAdded++;
                                addSpring = true;
                            }

                            if (addSpring)
                            {
                                combinedNewSprings++;
                                sb.Append('#');
                            }
                            else
                            {
                                sb.Append('?');
                            }
                        }

                        // This shift is valid if we added all new broken springs
                        if (newSpringsAdded == brokenSprings)
                        {
                            result.Add(new Group(sb.ToString()));
                        }
                    }
                    return result;
                }

                public override string ToString()
                {
                    return Data;
                }
            }


            public string Data { get; }
            public List<int> BrokenSprings { get; }

            public SpringRecord(string data, List<int> brokenSprings)
            {
                Data = data;
                BrokenSprings = brokenSprings;
            }

            public int Solve()
            {
                var groups = Data.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(g => new Group(g)).ToList();

                Debug.Assert(groups.Count <= BrokenSprings.Count);

                for (var i = 0; i < BrokenSprings.Count; i++)
                {
                    foreach (var group in groups)
                    {
                        var solvedGroups = group.Apply(BrokenSprings[i]);

                    }
                }

                return 0;
            }

            public override string ToString()
            {
                return $"{Data} {string.Join(',', BrokenSprings)}";
            }
        }
    }
}
