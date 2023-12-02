namespace Advent.Assignments
{
    internal class Day02_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            const int MaxRed = 12;
            const int MaxGreen = 13;
            const int MaxBlue = 14;
            var id = 1;
            var sum = 0;
            foreach (var game in input)
            {
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
                                if (num > MaxRed)
                                    goto impossible;
                                break;
                            case 'g':
                                if (num > MaxGreen)
                                    goto impossible;
                                break;
                            case 'b':
                                if (num > MaxBlue)
                                    goto impossible;
                                break;
                        }
                    }
                }

                sum += id;
            impossible:
                id++;
            }
            return sum.ToString();
        }
    }
}
