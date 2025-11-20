using System.Diagnostics;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Extensions;

namespace A3.RouteSearchGraphs.Domain.Algorithms;

internal sealed class AStarSearch : IGraphSearchAlgorithm
{
    private readonly HeuristicKind _heuristicKind;
    private readonly int _gridDimension;

    internal AStarSearch(HeuristicKind heuristicKind, int gridDimension)
    {
        _heuristicKind = heuristicKind;
        _gridDimension = gridDimension;
    }

    public ValueTask<SearchAlgorithmResult> Execute(int[][] matrix, int source, int target, CancellationToken cancellationToken = default)
    {
        matrix.EnsureValidIndex(source);
        matrix.EnsureValidIndex(target);

        var start = Stopwatch.GetTimestamp();

        var parents = Enumerable.Repeat(-1, matrix.Length).ToArray();

        var gScore = Enumerable.Repeat(int.MaxValue, matrix.Length).ToArray();

        var visited = new bool[matrix.Length];

        var queue = new PriorityQueue<int, double>();

        var expanded = 0;

        gScore[source] = 0;

        queue.Enqueue(source, Heuristics.Calculate(_heuristicKind, source, target, _gridDimension));

        while (queue.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var current = queue.Dequeue();

            if (visited[current])
            {
                continue;
            }

            visited[current] = true;

            expanded++;

            if (current == target)
            {
                break;
            }

            foreach (var (neighbor, weight) in matrix.EnumerateNeighbors(current))
            {
                if (visited[neighbor])
                {
                    continue;
                }

                if (gScore[current] == int.MaxValue)
                {
                    continue;
                }

                var tentativeG = gScore[current] + weight;

                if (tentativeG >= gScore[neighbor])
                {
                    continue;
                }

                gScore[neighbor] = tentativeG;
                parents[neighbor] = current;

                var priority = tentativeG + Heuristics.Calculate(_heuristicKind, neighbor, target, _gridDimension);

                queue.Enqueue(neighbor, priority);
            }
        }

        var delta = Stopwatch.GetElapsedTime(start);

        var path = new List<int>();

        if (gScore[target] != int.MaxValue)
        {
            path = parents.ReconstructPath(target);
        }

        var cost = matrix.CalculateCost(path);

        return ValueTask.FromResult(new SearchAlgorithmResult
        {
            Name = "A*",
            Heuristic = _heuristicKind.ToString(),
            Source = source,
            Target = target,
            Path = path,
            Cost = cost,
            ExpandedNodes = expanded,
            ExecutionTime = delta
        });
    }
}