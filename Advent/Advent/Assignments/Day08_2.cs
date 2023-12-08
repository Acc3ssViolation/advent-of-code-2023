using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day08_2 : IAssignment
    {
        string IAssignment.TestFile => "test-day08_2.txt";

        private record Node(string Name, string Left, string Right);

        public string Run(IReadOnlyList<string> input)
        {
            var dict = new Dictionary<string, Node>();

            var commands = input[0];

            var startPoints = new List<string>();

            foreach (var line in input.Skip(2))
            {
                var key = line.Substring(0, 3);
                var left = line.Substring(7, 3);
                var right = line.Substring(12, 3);

                if (key[2] == 'A')
                    startPoints.Add(key);

                dict.Add(key, new Node(key, left, right));
            }

            var stepsPerStart = new long[startPoints.Count];

            for (var i = 0; i < startPoints.Count; i++)
            {
                var steps = 0;
                var place = startPoints[i];
                var commandIndex = 0;
                while (place[2] != 'Z')
                {
                    var options = dict[place];
                    var command = commands[commandIndex++];
                    if (commandIndex >= commands.Length)
                        commandIndex = 0;
                    place = command == 'L' ? options.Left : options.Right;
                    steps++;
                }
                stepsPerStart[i] = steps;
            }
            var totalSteps = stepsPerStart.LowestCommonMultiple();

            return totalSteps.ToString();
        }

    }
}
