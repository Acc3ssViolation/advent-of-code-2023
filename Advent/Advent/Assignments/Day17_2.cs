
using Advent.Shared;

namespace Advent.Assignments
{
    internal class Day17_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var grid = new CharGrid(input);
            var nodes = new NodeProvider(grid);
            var path = PathFinding.Dijkstra(nodes);
            if (path == null)
                return "Oops";
            //RenderPath(grid, path);
            //Logger.DebugLine(grid.ToString());
            var totalLoss = path.Sum(n => n.HeatLoss);
            return totalLoss.ToString();
        }

        private static void RenderPath(CharGrid grid, List<Node> path)
        {
            foreach (var node in path)
            {
                if (node.HeatLoss == 0)
                    continue;

                char chr = node.Direction.ToDirection() switch
                {
                    Direction.North => '^',
                    Direction.East => '>',
                    Direction.South => 'v',
                    Direction.West => '<',
                    _ => '?'
                };
                grid[node.Point] = chr;
            }
        }

        private class Node
        {
            public int HeatLoss { get; }
            public Point Point { get; }
            public Point Direction { get; }
            public int StraightCount { get; }

            public Node(int heatLoss, Point point, Point direction, int straightCount)
            {
                HeatLoss = heatLoss;
                Point = point;
                Direction = direction;
                StraightCount = straightCount;
            }

            public override bool Equals(object? obj)
            {
                if (obj is not Node other)
                    return false;

                return other.Point == Point && other.Direction == Direction && other.StraightCount == StraightCount;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Point, Direction, StraightCount);
            }
        }

        private class NodeProvider : PathFinding.INodeProvider<Node>
        {
            private const int MaxStraight = 10;
            private const int MinStraight = 4;

            private CharGrid _grid;
            private Point _start;
            private Point _end;

            public NodeProvider(CharGrid grid)
            {
                _grid = grid ?? throw new ArgumentNullException(nameof(grid));
                _start = Point.Zero;
                _end = new Point(_grid.Width - 1, _grid.Height - 1);
            }

            public int GetDistance(Node node, Node neighbour)
            {
                return neighbour.HeatLoss;
            }

            public int GetHeuristic(Node node)
            {
                var distance = Math.Abs(_end.x - node.Point.x) + Math.Abs(_end.y - node.Point.y);
                return distance + node.HeatLoss;
            }

            public List<Node> GetNeighbours(Node node)
            {
                if (node.StraightCount == -1)
                {
                    // Start node, special neighbours
                    var left = Point.East;
                    var down = Point.South;
                    return new List<Node>(2)
                    {
                        new Node(_grid[left] - '0', left, Point.East, MaxStraight),
                        new Node(_grid[down] - '0', down, Point.South, MaxStraight),
                    };
                }

                var canTurn = node.StraightCount <= (MaxStraight - MinStraight + 1);
                var canStraight = node.StraightCount > 1;

                var neighbourCount = 0;
                if (canTurn)
                    neighbourCount += 2;
                if (canStraight)
                    neighbourCount += 1;
                var neighbours = new List<Node>(neighbourCount);

                if (canTurn)
                {
                    var leftDir = node.Direction.ToDirection().Left().ToPoint();
                    var rightDir = node.Direction.ToDirection().Right().ToPoint();
                    var leftPoint = node.Point + leftDir;
                    var rightPoint = node.Point + rightDir;

                    TryAddNode(neighbours, leftPoint, leftDir, MaxStraight);
                    TryAddNode(neighbours, rightPoint, rightDir, MaxStraight);
                }

                if (canStraight)
                {
                    var straightPoint = node.Point + node.Direction;
                    TryAddNode(neighbours, straightPoint, node.Direction, node.StraightCount - 1);
                }

                void TryAddNode(List<Node> neighbours, Point point, Point direction, int straightCount)
                {
                    if (point.x < 0 || point.y < 0 || point.x >= _grid.Width || point.y >= _grid.Height)
                        return;

                    var node = new Node(_grid[point] - '0', point, direction, straightCount);
                    neighbours.Add(node);
                }

                return neighbours;
            }

            public Node GetStart()
            {
                return new Node(0, _start, Point.North, -1);
            }

            public bool IsGoal(Node node)
            {
                return node.Point == _end;
            }
        }
    }
}