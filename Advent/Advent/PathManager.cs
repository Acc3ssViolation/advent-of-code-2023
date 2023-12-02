namespace Advent
{
    internal static class PathManager
    {
#if DEBUG
        private static readonly string BaseDir = "D:/Projects/advent-of-code-2023/Advent/Advent/";
#else
        private static readonly string BaseDir = AppContext.BaseDirectory;
#endif

        public static readonly string DataDirectory = Path.Combine(BaseDir, "Data");
    }
}
