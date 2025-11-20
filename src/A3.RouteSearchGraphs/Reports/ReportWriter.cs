using A3.RouteSearchGraphs.Domain.Abstractions;
using Microsoft.Extensions.Localization;

namespace A3.RouteSearchGraphs.Reports;

internal sealed class ReportWriter(IStringLocalizer<SharedResource> localizer)
{
    private readonly IStringLocalizer<SharedResource> _localizer = localizer;

    internal async ValueTask<string> WriteAsync(SearchAlgorithmResult result,
        string inputFileName,
        string outputDirectory,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(outputDirectory);

        var suffix = GetFileSuffix(result);

        var baseName = Path.GetFileName(inputFileName);

        var fileName = $"{baseName}.{suffix}";

        var outputPath = Path.Combine(outputDirectory, fileName);

        var report = new AlgorithmReportBuilder(_localizer)
            .WithAlgorithm(result.Name)
            .WithHeuristic(result.Heuristic)
            .WithSource(result.Source)
            .WithTarget(result.Target)
            .WithPath(result.Path)
            .WithCost(result.Cost)
            .WithExpandedNodes(result.ExpandedNodes)
            .WithTime(result.ExecutionTime)
            .Build();

        await File.WriteAllTextAsync(outputPath, report, cancellationToken);

        return outputPath;
    }

    private string GetFileSuffix(SearchAlgorithmResult result)
    {
        var heuristic = (result.Heuristic?.ToLowerInvariant() ?? string.Empty) switch
        {
            "euclidean" => _localizer["heuristic_kind_euclidean"].Value.ToLowerInvariant(),
            "manhattan" => _localizer["heuristic_kind_manhattan"].Value.ToLowerInvariant(),
            _ => string.Empty
        };

        return result.Name.ToLowerInvariant() switch
        {
            "bfs" => "bfs",
            "dfs" => "dfs",
            "dijkstra" => "dijkstra",
            "greedy best-first-search" => string.IsNullOrWhiteSpace(heuristic) ? "gbs" : $"gbs.{heuristic}",
            "a*" => string.IsNullOrWhiteSpace(heuristic) ? "a" : $"a.{heuristic}",
            _ => result.Name.Replace(' ', '_').ToLowerInvariant()
        };
    }
}