using System.Collections.Generic;
using System.Diagnostics;

namespace Advent.Assignments
{
    internal class Day05_2 : IAssignment
    {
        record MapRow(long Source, long Destination, long Length)
        {
            public long SourceEnd => Source + Length;
        }
        record SeedRange(long Start, long Length);
        record Range(long Start, long Length, Range? Parent, long Shift)
        {
            public long End => Start + Length;

            public override string ToString()
            {
                return $"[{Start} - {End - 1}] ({Length})";
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var maps = new List<List<MapRow>>();
            var wipMap = new List<MapRow>();
            var seeds = new List<SeedRange>();

            foreach (var line in input)
            {
                if (seeds.Count == 0)
                {
                    var seedData = line[7..].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();
                    for (var i = 0; i < seedData.Count; i += 2)
                    {
                        seeds.Add(new SeedRange(seedData[i], seedData[i + 1]));
                    }
                    continue;
                }

                if (line.Contains("map"))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (wipMap.Count > 0)
                        maps.Add(wipMap);
                    wipMap = new List<MapRow>();
                    continue;
                }

                var parts = line.Split(' ').Select(s => long.Parse(s)).ToArray();
                wipMap.Add(new MapRow(parts[1], parts[0], parts[2]));
            }

            if (wipMap.Count > 0)
                maps.Add(wipMap);

            var lowestLocation = long.MaxValue;
            foreach (var seed in seeds)
            {
                var seedRange = new Range(seed.Start, seed.Length, null, 0);
                var ranges = new List<Range>() { seedRange };
                foreach (var map in maps)
                {
                    var newRanges = new List<Range>();
                    foreach (var range in ranges)
                    {
                        newRanges.AddRange(Resolve(range, map));
                    }
                    ranges = newRanges;
                }
                if (ranges.Select(r => r.Length).Sum() != seedRange.Length)
                    Logger.ErrorLine($"Range integrity check failed");
                //Logger.DebugLine($"Split into {ranges.Count} ranges");
                var lowestLocationInSeedRange = ranges.Min(r => r.Start);
                if (lowestLocationInSeedRange < lowestLocation)
                    lowestLocation = lowestLocationInSeedRange;
            }

            return lowestLocation.ToString();
        }

        private static List<Range> Resolve(Range inputRange, List<MapRow> map)
        {
            var outputs = new List<Range>();
            var inputs = new Queue<Range>();

            //Logger.DebugLine($"Resolving range {inputRange}");

            inputs.Enqueue(inputRange);

            while (inputs.TryDequeue(out var input))
            {
                var foundRow = false;

                foreach (var row in map)
                {
                    if (row.Source >= input.End || input.Start >= row.SourceEnd)
                        continue;

                    //Logger.DebugLine($"\t{input} overlaps with {row}");

                    var start = Math.Max(row.Source, input.Start);
                    var end = Math.Min(row.SourceEnd, input.End);

                    var shift = row.Destination - row.Source;
                    var shifted = new Range(start + shift, end - start, input, shift);
                    //Logger.DebugLine($"\t\tOut: {shifted}");
                    outputs.Add(shifted);

                    if (input.Start < start)
                    {
                        var before = input with { Start = input.Start, Length = start - input.Start };
                        //Logger.DebugLine($"\t\tPre: {before}");
                        inputs.Enqueue(before);
                    }

                    if (input.End > end)
                    {
                        var after = input with { Start = end, Length = input.End - end };
                        //Logger.DebugLine($"\t\tPos: {after}");
                        inputs.Enqueue(after);
                    }

                    foundRow = true;
                    break;
                }

                if (!foundRow)
                {
                    // Pass it through in one piece
                    outputs.Add(input);
                    //Logger.DebugLine($"\tOut: {input}");
                }
            }

            return outputs;
        }

    }
}
