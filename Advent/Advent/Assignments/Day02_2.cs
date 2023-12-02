namespace Advent.Assignments
{
    internal class Day02_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sum = 0;
            Span<int> maxColors = stackalloc int[3];
            foreach (var game in input)
            {
                maxColors[0] = 0;
                maxColors[1] = 0;
                maxColors[2] = 0;

                var reveals = game.Substring(game.IndexOf(':') + 1).Split(';');
                foreach (var reveal in reveals)
                {
                    var colors = reveal.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    foreach (var color in colors)
                    {
                        var parts = color.Split(' ');
                        var num = int.Parse(parts[0], System.Globalization.NumberStyles.None);
                        var colorId = parts[1];
                        switch (colorId[0])
                        {
                            case 'r':
                                if (num > maxColors[0])
                                    maxColors[0] = num;
                                break;
                            case 'g':
                                if (num > maxColors[1])
                                    maxColors[1] = num;
                                break;
                            case 'b':
                                if (num > maxColors[2])
                                    maxColors[2] = num;
                                break;
                        }
                    }
                }

                var power = maxColors[0] * maxColors[1] * maxColors[2];
                sum += power;
            }
            return sum.ToString();
        }
    }
}
