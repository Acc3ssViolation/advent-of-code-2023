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
    }
}
