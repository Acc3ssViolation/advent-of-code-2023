
using Advent.Shared;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Advent.Assignments
{
    internal class Day12_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sum = 0L;
            foreach (var item in input)
            {
                var parts = item.Split(' ');
                var recordBaseLength = parts[0].Length;
                var recordBuffer = new char[recordBaseLength * 5 + 4];
                var recordSpan = recordBuffer.AsSpan();
                parts[0].CopyTo(recordSpan.Slice(0));
                parts[0].CopyTo(recordSpan.Slice(recordBaseLength + 1));
                parts[0].CopyTo(recordSpan.Slice((recordBaseLength + 1) * 2));
                parts[0].CopyTo(recordSpan.Slice((recordBaseLength + 1) * 3));
                parts[0].CopyTo(recordSpan.Slice((recordBaseLength + 1) * 4));
                recordSpan[(recordBaseLength + 1) * 1 - 1] = '?';
                recordSpan[(recordBaseLength + 1) * 2 - 1] = '?';
                recordSpan[(recordBaseLength + 1) * 3 - 1] = '?';
                recordSpan[(recordBaseLength + 1) * 4 - 1] = '?';

                var record = new string(recordBuffer);
                var baseGroups = parts[1].ExtractInts();
                var groups = baseGroups.Repeat(5).ToList();
                var options = Day12_1.RecursionMem(record, groups);
                sum += options;
                //Logger.DebugLine($"{record} {string.Join(',', groups)} has {options} options");
            }
            return sum.ToString();
        }
    }
}
