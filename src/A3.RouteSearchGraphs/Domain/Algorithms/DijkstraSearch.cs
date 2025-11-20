using System.Diagnostics;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Extensions;

namespace A3.RouteSearchGraphs.Domain.Algorithms;

internal sealed class DijkstraSearch : IGraphSearchAlgorithm
{
    public ValueTask<SearchAlgorithmResult> Execute(int[][] matrix, int source, int target, CancellationToken cancellationToken = default)
    {
        matrix.EnsureValidIndex(source);
        matrix.EnsureValidIndex(target);

        var start = Stopwatch.GetTimestamp();

        var parents = Enumerable.Repeat(-1, matrix.Length).ToArray();

        var distances = Enumerable.Repeat(int.MaxValue, matrix.Length).ToArray();

        var queue = new PriorityQueue<int, int>();

        var expanded = 0;

        distances[source] = 0;

        queue.Enqueue(source, 0);

        while (queue.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var current = queue.Dequeue();

            if (distances[current] == int.MaxValue)
            {
                continue;
            }

            expanded++;

            if (current == target)
            {
                break;
            }

            foreach (var (neighbor, weight) in matrix.EnumerateNeighbors(current))
            {
                var candidate = distances[current] + weight;

                if (candidate >= distances[neighbor])
                {
                    continue;
                }

                distances[neighbor] = candidate;
                parents[neighbor] = current;

                queue.Enqueue(neighbor, candidate);
            }
        }

        var delta = Stopwatch.GetElapsedTime(start);

        var path = new List<int>();

        if (distances[target] != int.MaxValue)
        {
            path = parents.ReconstructPath(target);
        }

        var cost = matrix.CalculateCost(path);

        return ValueTask.FromResult(new SearchAlgorithmResult
        {
            Name = "Dijkstra",
            Source = source,
            Target = target,
            Path = path,
            Cost = cost,
            ExpandedNodes = expanded,
            ExecutionTime = delta
        });
    }
}