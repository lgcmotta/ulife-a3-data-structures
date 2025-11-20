using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace A3.RouteSearchGraphs.Columns;

public class IterationCountColumn : IColumn
{
    public string Id => nameof(IterationCountColumn);

    public string ColumnName => "Iterations";

    public string Legend => "Iterations Count";

    public bool AlwaysShow => true;

    public bool IsNumeric => true;

    public int PriorityInCategory => 0;

    public ColumnCategory Category => ColumnCategory.Custom;

    public UnitType UnitType => UnitType.Dimensionless;


    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public bool IsAvailable(Summary summary) => true;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
        => GetValue(summary, benchmarkCase);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        var report = summary.Reports.First(br => br.BenchmarkCase == benchmarkCase);

        return report.ResultStatistics?.N.ToString() ?? "?";
    }
}