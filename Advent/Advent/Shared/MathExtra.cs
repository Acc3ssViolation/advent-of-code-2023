using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Shared
{
    internal static class MathExtra
    {
        public static long LowestCommonMultiple(this IEnumerable<long> nums)
        {
            return nums.Aggregate(LowestCommonMultiple);
        }

        public static long LowestCommonMultiple(long a, long b)
        {
            return Math.Abs(a * b) / GreatestCommonDivisor(a, b);
        }

        public static long GreatestCommonDivisor(long a, long b)
        {
            if (b == 0)
                return a;
            return GreatestCommonDivisor(b, a % b);
        }

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public static T[] Repeat<T>(this List<T> list, int count)
        {
            var result = new T[count * list.Count];
            for (var i = 0; i < count; i++)
            {
                list.CopyTo(result, list.Count * i);
            }
            return result;
        }

        public static int UniquePairCount(int count)
        {
            return count * (count - 1) / 2;
        }
    }
}
