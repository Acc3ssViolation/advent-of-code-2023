using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day15_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            return Hash(input[0]).ToString();
        }

        private static int Hash(string input)
        {
            var sum = 0;
            var hash = 0;

            for (var i = 0; i < input.Length; i++)
            {
                var chr = input[i];
                if (chr == ',')
                {
                    sum += hash;
                    hash = 0;
                }
                else
                {
                    hash += input[i];
                    hash *= 17;
                    hash &= 0xFF;
                }
            }

            return sum + hash;
        }
    }
}
