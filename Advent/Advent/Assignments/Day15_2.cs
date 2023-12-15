using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day15_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            return Process(input[0]).ToString();
        }

        private record Lens(string Label, int FocalLength)
        {
            public override string ToString()
            {
                return $"[{Label} {FocalLength}]";
            }
        }

        private static int Process(string input)
        {
            var hash = 0;
            var startIndex = 0;
            var boxes = new List<Lens>[256];

            for (var i = 0; i < input.Length - 1; i++)
            {
                var chr = input[i];
                if (chr == '=')
                {
                    var focalLength = input[i + 1] - '0';
                    var box = hash;
                    var list = boxes[box];
                    if (list == null)
                    {
                        list = new List<Lens>();
                        boxes[box] = list;
                    }
                    var label = input.Substring(startIndex, i - startIndex);
                    var lensIndex = list.FindIndex(l => l.Label == label);
                    if (lensIndex < 0)
                        list.Add(new Lens(label, focalLength));
                    else
                        list[lensIndex] = new Lens(label, focalLength);

                    DumpBoxes(boxes);
                }
                else if (chr == '-')
                {
                    var box = hash;
                    var list = boxes[box];
                    if (list != null)
                    {
                        var label = input.Substring(startIndex, i - startIndex);
                        list.RemoveAll(l => l.Label == label);
                    }

                    DumpBoxes(boxes);
                }
                else if (chr == ',')
                {
                    hash = 0;
                    startIndex = i + 1;
                }
                else
                {
                    hash += input[i];
                    hash *= 17;
                    hash &= 0xFF;
                }
            }

            // Find final thing
            var sum = 0;
            for (var i = 0; i < boxes.Length; i++)
            {
                var box = boxes[i];
                if (box == null) 
                    continue;

                for (var j = 0; j < box.Count; j++)
                {
                    var lens = box[j];
                    var power = (j + 1) * (i + 1) * lens.FocalLength;
                    sum += power;
                }
            }

            return sum;
        }

        private static void DumpBoxes(List<Lens>[] boxes)
        {
            return;
            for (var i = 0; i < boxes.Length; i++)
            {
                var box = boxes[i];
                if (box == null)
                    continue;
                Logger.DebugLine($"Box {i}: {string.Join(' ', box)}");
            }

            Logger.DebugLine("====================");
        }
    }
}
