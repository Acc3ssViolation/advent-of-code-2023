using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day09_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var total = 0;
            foreach (var item in input)
            {
                var index = 0;
                var sequence = new List<int>();
                while (index < item.Length)
                {
                    sequence.Add(ParseUtils.ParseInt(item, ref index));
                }

                //Logger.DebugLine($"Sequence [{string.Join(' ', sequence)}]");

                checked
                {
                    var factors = GetFirstFactors(sequence);
                    var result = GetNextValue(sequence[0], factors);
                    total += result;
                }
            }
            return total.ToString();
        }

        private int GetNextValue(int lastInSequence, List<int> factors)
        {
            checked
            {
                var previousFactor = 0;
                for (var i = factors.Count - 1; i >= 0; i--)
                {
                    previousFactor = factors[i] - previousFactor;
                }
                return lastInSequence - previousFactor;
            }
        }

        private List<int> GetFirstFactors(List<int> sequence)
        {
            var factors = new List<int>();
            var sequenceToDelta = sequence;
            while (GetDifferences(sequenceToDelta, out var deltas))
            {
                factors.Add(deltas[0]);
                sequenceToDelta = deltas;
            }
            return factors;
        }

        private bool GetDifferences(List<int> sequence, out List<int> result)
        {
            result = new List<int>(sequence.Count + 1);
            var isNotZero = false;
            for (var i = 0; i < sequence.Count - 1; i++)
            {
                var delta = sequence[i + 1] - sequence[i];
                result.Add(delta);
                if (delta != 0)
                    isNotZero = true;
            }
            return isNotZero;
        }
    }
}
