using System.Runtime.CompilerServices;

namespace A3.RouteSearchGraphs.Domain.Extensions;

internal static class GraphExtensions
{
    extension(int[][] matrix)
    {
        internal IEnumerable<(int neighbor, int weight)> EnumerateNeighbors(int node)
        {
            var row = matrix[node];

            for (var index = 0; index < row.Length; index++)
            {
                var weight = row[index];

                if (weight > 0)
                {
                    yield return (index, weight);
                }
            }
        }

        internal int? CalculateCost(IReadOnlyList<int> path)
        {
            if (path is { Count: <= 1 })
            {
                return path is { Count: 1 } ? 0 : null;
            }

            var cost = 0;

            for (var index = 0; index < path.Count - 1; index++)
            {
                var from = path[index];

                var to = path[index + 1];

                var weight = matrix[from][to];

                if (weight <= 0)
                {
                    return null;
                }

                cost += weight;
            }

            return cost;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void EnsureValidIndex(int value, [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        {
            var maxExclusive = matrix.Length;

            if (value < 0 || value >= maxExclusive)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"Value '{value}' is out of bounds for graph with {maxExclusive} nodes.");
            }
        }
    }

    extension(int[] parents)
    {
        internal List<int> ReconstructPath(int destination)
        {
            var path = new List<int>();

            var current = destination;

            while (current != -1)
            {
                path.Add(current);

                current = parents[current];
            }

            path.Reverse();

            return path;
        }
    }
}