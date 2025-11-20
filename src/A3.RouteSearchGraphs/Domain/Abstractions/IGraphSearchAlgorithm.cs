namespace A3.RouteSearchGraphs.Domain.Abstractions;

internal interface IGraphSearchAlgorithm
{
    internal ValueTask<SearchAlgorithmResult> Execute(
        int[][] matrix,
        int source,
        int target,
        CancellationToken cancellationToken = default
    );
}