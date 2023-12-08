namespace Advent.Assignments
{
    internal class Day08_1 : IAssignment
    {
        private record Node(string Name, string Left, string Right);

        public string Run(IReadOnlyList<string> input)
        {
            var dict = new Dictionary<string, Node>();

            var commands = input[0];

            foreach (var line in input.Skip(2))
            {
                var key = line.Substring(0, 3);
                var left = line.Substring(7, 3);
                var right = line.Substring(12, 3);
                
                dict.Add(key, new Node(key, left, right));
            }

            var steps = 0;

            var place = "AAA";
            var end = "ZZZ";

            var commandIndex = 0;
            while (!string.Equals(place, end, StringComparison.OrdinalIgnoreCase))
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
