namespace A3.RouteSearchGraphs.Reports;

public readonly ref struct Defaults
{
    internal static readonly string DefaultOutputDirectory =
        Path.Combine(Directory.GetCurrentDirectory(), "reports");
}