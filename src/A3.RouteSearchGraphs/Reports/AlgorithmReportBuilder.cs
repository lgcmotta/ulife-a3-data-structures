using System.Globalization;
using System.Text;
using A3.RouteSearchGraphs.Extensions;
using Microsoft.Extensions.Localization;

namespace A3.RouteSearchGraphs.Reports;

internal class AlgorithmReportBuilder(IStringLocalizer<SharedResource> localizer)
{
    private readonly IStringLocalizer<SharedResource> _localizer = localizer;
    private readonly Dictionary<string, Action<StringBuilder>> _actions = [];

    private readonly string[] _steps =
    [
        nameof(WithAlgorithm),
        nameof(WithHeuristic),
        nameof(WithSource),
        nameof(WithTarget),
        nameof(WithPath),
        nameof(WithCost),
        nameof(WithExpandedNodes),
        nameof(WithTime)
    ];

    internal AlgorithmReportBuilder WithAlgorithm(string algorithm)
    {
        _actions.AddOrUpdate(nameof(WithAlgorithm),
            builder => builder.AppendLine($"{_localizer["algorithm"].Value}: {algorithm}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithHeuristic(string? heuristic = null)
    {
        var key = _localizer["heuristic"].Value;

        var normalized = (heuristic ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "euclidean" => _localizer["heuristic_kind_euclidean"].Value.ToTitleCase().ToLeftSpacedString(),
            "manhattan" => _localizer["heuristic_kind_manhattan"].Value.ToTitleCase().ToLeftSpacedString(),
            _ => string.Empty
        };

        _actions.AddOrUpdate(nameof(WithHeuristic),
            builder => builder.AppendLine($"{key}:{normalized}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithSource(int source)
    {
        _actions.AddOrUpdate(nameof(WithSource),
            builder => builder.AppendLine($"{_localizer["source"].Value}: {source}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithTarget(int target)
    {
        _actions.AddOrUpdate(nameof(WithTarget),
            builder => builder.AppendLine($"{_localizer["target"].Value}: {target}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithPath(IReadOnlyList<int> nodes)
    {
        var path = nodes is { Count: > 0 } ? string.Join(" -> ", nodes).ToLeftSpacedString() : string.Empty;

        _actions.AddOrUpdate(nameof(WithPath),
            builder => builder.AppendLine($"{_localizer["path"].Value}:{path}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithCost(int? cost)
    {
        var value = cost is not null
            ? cost.Value.ToString(CultureInfo.InvariantCulture).ToLeftSpacedString()
            : string.Empty;

        _actions.AddOrUpdate(nameof(WithCost),
            builder => builder.AppendLine($"{_localizer["cost"].Value}:{value}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithExpandedNodes(int? expandedNodes)
    {
        var value = expandedNodes?.ToString(CultureInfo.InvariantCulture).ToLeftSpacedString() ?? string.Empty;

        _actions.AddOrUpdate(nameof(WithExpandedNodes),
            builder => builder.AppendLine($"{_localizer["expanded_nodes"].Value}:{value}")
        );

        return this;
    }

    internal AlgorithmReportBuilder WithTime(TimeSpan time)
    {
        _actions.AddOrUpdate(nameof(WithTime),
            builder => builder.AppendLine($"{_localizer["time"].Value} (ms): {time.TotalMilliseconds.ToString("F4", CultureInfo.InvariantCulture)}")
        );

        return this;
    }

    internal string Build()
    {
        var builder = new StringBuilder();

        foreach (var step in _steps)
        {
            if (_actions.TryGetValue(step, out var action))
            {
                action(builder);
            }
        }

        return builder.ToString();
    }
}