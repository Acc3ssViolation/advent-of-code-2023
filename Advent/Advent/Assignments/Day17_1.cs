
namespace Advent.Assignments
{
    internal class Day17_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            return "";
        }

        public interface INodeProvider<T>
        {
            T GetStart();
            bool IsGoal(T node);
            List<T> GetNeighbours(T node);
            int GetDistance(T node, T neighbour);
            int GetHeuristic(T node);
        }

        public List<T>? AStar<T>(INodeProvider<T> nodeProvider) where T:notnull
        {
            var openSet = new PriorityQueue<T, int>();
            var gScore = new Dictionary<T, int>();
            var cameFrom = new Dictionary<T, T>();
            var start = nodeProvider.GetStart();
            openSet.Enqueue(start, 0);
            gScore[start] = 0;
            while (openSet.TryDequeue(out var node, out var _))
            {
                if (nodeProvider.IsGoal(node))
                {
                    var path = new List<T>();
                    while (node != null)
                    {
                        path.Add(node);
                        node = cameFrom[node];
                    }
                    return path;
                }

                var neighbours = nodeProvider.GetNeighbours(node);
                foreach (var neighbour in neighbours)
                {
                    var tgScore = gScore[node] + nodeProvider.GetDistance(node, neighbour);
                    if (!gScore.TryGetValue(neighbour, out var gScoreNeighbour))
                    {
                        gScoreNeighbour = int.MaxValue;
                    }
                    if (tgScore < gScoreNeighbour)
                    {
                        cameFrom[neighbour] = node;
                        gScore[neighbour] = tgScore;
                        var fScore = tgScore + nodeProvider.GetHeuristic(neighbour);
                        // TODO: Only do this if not queued already
                        if (!openSet.UnorderedItems.Contains((neighbour, fScore)))
                            openSet.Enqueue(neighbour, fScore);
                    }
                }
            }

            return null;
        }
    }
}