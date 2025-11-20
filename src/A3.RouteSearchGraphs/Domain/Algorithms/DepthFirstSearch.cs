using System.Diagnostics;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Extensions;

namespace A3.RouteSearchGraphs.Domain.Algorithms;

internal sealed class DepthFirstSearch : IGraphSearchAlgorithm
{
    public ValueTask<SearchAlgorithmResult> Execute(int[][] matrix, int source, int target, CancellationToken cancellationToken = default)
    {
        matrix.EnsureValidIndex(source);
        matrix.EnsureValidIndex(target);

        var start = Stopwatch.GetTimestamp();

        var parents = Enumerable.Repeat(-1, matrix.Length).ToArray();

        var visited = new bool[matrix.Length];

        var stack = new Stack<int>();

        var expanded = 0;

        stack.Push(source);

        while (stack.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var current = stack.Pop();

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

            foreach (var (neighbor, _) in matrix.EnumerateNeighbors(current).OrderByDescending(n => n.neighbor))
            {
                if (visited[neighbor])
                {
                    continue;
                }

                parents[neighbor] = current;
                stack.Push(neighbor);
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
            Name = "DFS",
            Source = source,
            Target = target,
            Path = path,
            Cost = cost,
            ExpandedNodes = expanded,
            ExecutionTime = delta
        });
    }
}