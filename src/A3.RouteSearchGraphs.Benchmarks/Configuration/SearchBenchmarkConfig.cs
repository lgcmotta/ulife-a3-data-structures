using A3.RouteSearchGraphs.Columns;
using BenchmarkDotNet.Configs;

namespace A3.RouteSearchGraphs.Configuration;

public class SearchBenchmarkConfig : ManualConfig
{
    public SearchBenchmarkConfig()
    {
        WithArtifactsPath("./artifacts");
        AddColumn(new MeanMillisecondsColumn());
        AddColumn(new IterationCountColumn());
    }
}