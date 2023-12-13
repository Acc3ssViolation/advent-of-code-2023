
using Advent.Shared;
using System.Diagnostics;
using System.Text;

namespace Advent.Assignments
{
    internal class Day12_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var total = 0;
            foreach (var line in input)
            {
                var parts = line.Split(' ');
                var compactGroup = parts[1].ExtractInts();
                var groups = new int[compactGroup.Count];// * 5];
                compactGroup.CopyTo(groups, 0);
                //compactGroup.CopyTo(groups, compactGroup.Count);
                //compactGroup.CopyTo(groups, compactGroup.Count * 2);
                //compactGroup.CopyTo(groups, compactGroup.Count * 3);
                //compactGroup.CopyTo(groups, compactGroup.Count * 4);

                var record = parts[0];// + "?" + parts[0] + "?" + parts[0] + "?" + parts[0] + "?" + parts[0];

                var recordTotal = DoThingRecursively(record, 0, groups);
                Logger.DebugLine($"\t{record} => {recordTotal}");
                //var states = GeneratePossibleStates(record).ToList();
                //Logger.DebugLine($"{states.Count} states for {record}");
                //var validCount = 0;
                //foreach (var state in states)
                //{
                //    //Logger.DebugLine($"\t{record} => {state}");
                //    if (Validate(state, groups))
                //        validCount++;
                //}
                //total += validCount;
                ////Logger.DebugLine($"{line} => {validCount}");
            }

            return total.ToString();
        }

        private static bool Validate(string state, IReadOnlyList<int> groups)
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

        private static int DoThingRecursively(string input, int startOffset, ReadOnlySpan<int> groups)
        {
            //Logger.DebugLine($"{new string(' ', startOffset)}{input} {string.Join(',', groups.ToArray())}");
            if (input.Length == 0 && groups.Length == 0)
                return 0;

            var totalCount = 0;

            for (var shift = 0; shift < input.Length; shift++)
            {
                // Try to see if we can fit these ranges with a bit of a shift
                var ranges = FitRanges(input, shift, groups);
                // No? Then we're done
                if (ranges.Length == 0)
                    break;

                // Yes! See if this combination is valid                
                if (groups.Length == 1)
                    return IsValid(ranges, input) ? 1 : 0;

                var nextInput = input[(ranges[0].end + 1)..];
                var nextGroups = groups[1..];

                totalCount += DoThingRecursively(nextInput, shift + startOffset, nextGroups);
            }
            return totalCount;
        }

        private static bool IsValid(LineRange[] ranges, string input)
        {
            for (int i = 0, r = 0; i < input.Length; i++)
            {
                if (input[i] != '#')
                    continue;
                while (ranges[r].end <= i)
                    r++;
                if (ranges[r].start > i)
                    return false;
            }
            Logger.DebugLine("");
            return true;
        }

        private static LineRange[] FitRanges(string input, int start, ReadOnlySpan<int> groups)
        {
            // Get the initial position for each range
            var xCount = start;
            var ranges = new LineRange[groups.Length];
            for (var i = 0; i < groups.Length; i++)
            {
                var rangeWidth = groups[i];
                // Find the first place this could possibly fit
                xCount = FindNextFreeSlot(input, xCount, rangeWidth);
                if (xCount < 0)
                    return Array.Empty<LineRange>();

                var range = new LineRange(xCount, rangeWidth);
                ranges[i] = range;
                // One extra space, ranges are not allowed to be adjacent
                xCount += rangeWidth + 1;
            }

            return ranges;
        }

        private static int FindNextFreeSlot(string input, int start, int width)
        {
            var counter = 0;
            for (var i = start; i < input.Length; i++)
            {
                var chr = input[i];
                if (chr == '?' || chr == '#')
                {
                    counter++;
                    if (counter >= width)
                        return i - width + 1;
                }
                else
                {
                    counter = 0;
                }
            }

            return -1;
        }

        record StateMeta(int MatchedGroups);

        //private static IEnumerable<string> GeneratePossibleStates(string prefix, string input, StateMeta meta, IReadOnlyList<int> groups)
        //{
        //    var minGroupSize = groups.Skip(meta.MatchedGroups).Min();
        //    var maxGroupSize = groups.Skip(meta.MatchedGroups).Max();

        //    var unknownCount = 0;
        //    for (var i = 0; i < input.Length; i++)
        //    {
        //        if (input[i] == '?')
        //        {
        //            unknownCount++;
        //            if (unknownCount >= minGroupSize)
        //            {
        //                // Allow randomizing this
        //            }
        //            else
        //            {
        //                // MUST be '.'
        //            }
        //        }
        //    }

        //    if (input.Length == 0)
        //    {
        //        yield return input;
        //        yield break;
        //    }

        //    if (input[0] == '?')
        //    {
        //        foreach (var result in GeneratePossibleStates(input.Substring(1)))
        //        {
        //            yield return "." + result;
        //            yield return "#" + result;
        //        }
        //    }
        //    else
        //    {
        //        foreach (var result in GeneratePossibleStates(input.Substring(1)))
        //        {
        //            yield return input[0] + result;
        //        }
        //    }
        //}
    }
}
