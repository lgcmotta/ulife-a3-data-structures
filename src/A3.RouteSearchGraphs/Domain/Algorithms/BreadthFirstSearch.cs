using System.Diagnostics;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Extensions;

namespace A3.RouteSearchGraphs.Domain.Algorithms;

internal sealed class BreadthFirstSearch : IGraphSearchAlgorithm
{
    public ValueTask<SearchAlgorithmResult> Execute(int[][] matrix, int source, int target, CancellationToken cancellationToken = default)
    {
        matrix.EnsureValidIndex(source);
        matrix.EnsureValidIndex(target);

        var start = Stopwatch.GetTimestamp();

        var parents = Enumerable.Repeat(-1, matrix.Length).ToArray();

        var visited = new bool[matrix.Length];

        var queue = new Queue<int>();

        var expanded = 0;

        visited[source] = true;

        queue.Enqueue(source);

        while (queue.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var current = queue.Dequeue();

            expanded++;

            if (current == target)
            {
                break;
            }

            foreach (var (neighbor, _) in matrix.EnumerateNeighbors(current))
            {
                if (visited[neighbor])
                {
                    continue;
                }

                visited[neighbor] = true;
                parents[neighbor] = current;

                queue.Enqueue(neighbor);
            }
        }

        var delta = Stopwatch.GetElapsedTime(start);

        var path = new List<int>();

        if (visited[target])
        {
            path = parents.ReconstructPath(target);
        }

        var cost = matrix.CalculateCost(path);

        return ValueTask.FromResult(new SearchAlgorithmResult
        {
            Name = "BFS",
            Source = source,
            Target = target,
            Path = path,
            Cost = cost,
            ExpandedNodes = expanded,
            ExecutionTime = delta
        });
    }
}