using Advent.Shared;
using System.Text;

namespace Advent.Assignments
{
    public class CharGrid
    {
        public int Width { get; }
        public int Height { get; }

        public char[] Chars => _data;

        private readonly char[] _data;

        public char this[Point point]
        {
            get => _data[point.x + point.y * Width];
            set => _data[point.x + point.y * Width] = value;
        }

        public CharGrid(int width, int height)
        {
            Width = width;
            Height = height;
            _data = new char[Width * Height];
        }

        public CharGrid(IReadOnlyList<string> strings)
        {
            Width = strings[0].Length;
            Height = strings.Count;
            _data = new char[Width * Height];
            for (var i = 0; i < strings.Count; i++)
            {
                strings[i].CopyTo(0, _data, i * Width, Width);
            }
        }

        public Point Find(char chr)
        {
            for (var y = 0; y < Height; y++)
                for (var x = 0; x < Width; x++)
                {
                    if (_data[y * Width + x] == chr)
                        return new Point(x, y);
                }
            return default;
        }

        public override string ToString()
        {
            var nl = Environment.NewLine.Length;
            var sb = new char[((Width + nl) * Height)];

            for (var y = 0; y < Height; y++)
            {
                _data.AsSpan(y * Width, Width).CopyTo(sb.AsSpan(y * (Width + nl), Width));
                Environment.NewLine.CopyTo(sb.AsSpan(y * (Width + nl) + Width, nl));
            }
            return new string(sb);
        }
    }
}
