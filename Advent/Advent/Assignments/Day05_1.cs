namespace Advent.Assignments
{
    internal class Day05_1 : IAssignment
    {
        record MapRow(long Source, long Destination, long Length);

        public string Run(IReadOnlyList<string> input)
        {
            var maps = new List<List<MapRow>>();
            var wipMap = new List<MapRow>();
            var seeds = new List<long>();

            foreach (var line in input)
            {
                if (seeds.Count == 0)
                {
                    seeds = line[7..].Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(s => long.Parse(s)).ToList();
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
                var location = ResolveAll(seed, maps);
                if (location < lowestLocation)
                    lowestLocation = location;
            }

            return lowestLocation.ToString();
        }

        private static long ResolveAll(long value, List<List<MapRow>> maps)
        {
            foreach (var map in maps)
            {
                var nextValue = Resolve(value, map);
                //Logger.DebugLine($"{value} -> {nextValue}");
                value = nextValue;
            }
            return value;
        }

        private static long Resolve(long value, List<MapRow> map)
        {
            foreach (var row in map)
            {
                var offset = value - row.Source;
                if (offset >= 0 && offset < row.Length)
                    return row.Destination + offset;
            }
            return value;
        }
    }
}
