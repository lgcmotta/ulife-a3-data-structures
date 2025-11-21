using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace A3.RouteSearchGraphs.Columns;

public class MeanMillisecondsColumn : IColumn
{
    public string Id => nameof(MeanMillisecondsColumn);

    public string ColumnName => "Mean (ms)";

    public string Legend => "Mean (milliseconds)";

    public bool AlwaysShow => true;

    public bool IsNumeric => true;

    public int PriorityInCategory => 0;

    public ColumnCategory Category => ColumnCategory.Custom;

    public UnitType UnitType => UnitType.Time;


    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public bool IsAvailable(Summary summary) => true;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        => GetValue(summary, benchmarkCase);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var stats = summary.Reports.First(br => br.BenchmarkCase == benchmarkCase).ResultStatistics;

        return stats is not null ? (stats.Mean / 1_000_000.0D).ToString("F4") : "?";
    }
}