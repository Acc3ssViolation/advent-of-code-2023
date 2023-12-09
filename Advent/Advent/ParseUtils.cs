namespace Advent
{
    internal static class ParseUtils
    {
        public static long ToLongIgnoreWhitespace(this ReadOnlySpan<char> str)
        {
            var num = 0L;
            foreach (var chr in str)
            {
                if (!char.IsNumber(chr))
                {
                    continue;
                }
                num *= 10;
                num += chr - '0';
            }
            return num;
        }

        public static int ParseIntPositive(string str, ref int index)
        {
            var num = 0;
            while (index < str.Length)
            {
                var chr = str[index];
                if (!char.IsNumber(chr))
                {
                    break;
                }
                num *= 10;
                num += chr - '0';
                index++;
            }
            return num;
        }

        public static int ParseInt(string str, ref int index)
        {
            bool negative = false;
            var num = 0;
            while (index < str.Length && !char.IsNumber(str[index]) && str[index] != '-')
            {
                index++;
            }
            if (str[index] == '-')
            {
                negative = true;
                index++;
            }
            while (index < str.Length)
            {
                var chr = str[index];
                if (!char.IsNumber(chr))
                {
                    break;
                }
                num *= 10;
                num += chr - '0';
                index++;
            }
            return negative ? -num : num;
        }
    }
}
