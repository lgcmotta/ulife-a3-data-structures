namespace A3.RouteSearchGraphs.Domain.Abstractions;

public record SearchAlgorithmResult
{
    public required string Name { get; init; }

    public string? Heuristic { get; init; } = string.Empty;

    public required int Source { get; init; }

    public required int Target { get; init; }

    public List<int> Path { get; init; } = [];

    public int? Cost { get; init; }

    public int? ExpandedNodes { get; init; }

    public required TimeSpan ExecutionTime { get; init; }
}