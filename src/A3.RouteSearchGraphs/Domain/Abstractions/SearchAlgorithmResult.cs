namespace A3.RouteSearchGraphs.Domain.Abstractions;

internal record SearchAlgorithmResult
{
    internal required string Name { get; init; }

    internal string? Heuristic { get; init; } = string.Empty;

    internal required int Source { get; init; }

    internal required int Target { get; init; }

    internal List<int> Path { get; init; } = [];

    internal int? Cost { get; init; }

    internal int? ExpandedNodes { get; init; }

    internal required TimeSpan ExecutionTime { get; init; }
}