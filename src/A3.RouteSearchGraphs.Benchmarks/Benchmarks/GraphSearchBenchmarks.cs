using A3.RouteSearchGraphs.Configuration;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Extensions;
using BenchmarkDotNet.Attributes;

namespace A3.RouteSearchGraphs.Benchmarks;

[MemoryDiagnoser, RankColumn, CategoriesColumn, Config(typeof(SearchBenchmarkConfig))]
public partial class GraphSearchBenchmarks
{
    private Dictionary<string, int[][]> _files = [];
    private Dictionary<(string, HeuristicKind), int> _dimensions = [];

    private const string Matrix3X3Txt = "matrix_3x3.txt";
    private const string Matrix4X4Txt = "matrix_4x4.txt";
    private const string Matrix16X16Txt = "matrix_16x16.txt";
    private const string Matrix32X32Txt = "matrix_32x32.txt";
    private const string Matrix64X64Txt = "matrix_64x64.txt";

    [GlobalSetup]
    public async ValueTask SetupAsync()
    {
        const string matrix3X3 = "./samples/matrix_3x3.txt";
        const string matrix4X4 = "./samples/matrix_4x4.txt";
        const string matrix16X16 = "./samples/matrix_16x16.txt";
        const string matrix32X32 = "./samples/matrix_32x32.txt";
        const string matrix64X64 = "./samples/matrix_64x64.txt";

        _files = new Dictionary<string, int[][]>
        {
            { Matrix3X3Txt, await matrix3X3.ParseAdjacencyMatrix() },
            { Matrix4X4Txt, await matrix4X4.ParseAdjacencyMatrix() },
            { Matrix16X16Txt, await matrix16X16.ParseAdjacencyMatrix() },
            { Matrix32X32Txt, await matrix32X32.ParseAdjacencyMatrix() },
            { Matrix64X64Txt, await matrix64X64.ParseAdjacencyMatrix() }
        };

        _dimensions = new Dictionary<(string, HeuristicKind), int>
        {
            { (Matrix3X3Txt, HeuristicKind.Manhattan), Heuristics.ResolveGridDimension(_files[Matrix3X3Txt].Length) },
            { (Matrix3X3Txt, HeuristicKind.Euclidean), Heuristics.ResolveGridDimension(_files[Matrix3X3Txt].Length) },
            { (Matrix4X4Txt, HeuristicKind.Manhattan), Heuristics.ResolveGridDimension(_files[Matrix4X4Txt].Length) },
            { (Matrix4X4Txt, HeuristicKind.Euclidean), Heuristics.ResolveGridDimension(_files[Matrix4X4Txt].Length) },
            { (Matrix16X16Txt, HeuristicKind.Manhattan), Heuristics.ResolveGridDimension(_files[Matrix16X16Txt].Length) },
            { (Matrix16X16Txt, HeuristicKind.Euclidean), Heuristics.ResolveGridDimension(_files[Matrix16X16Txt].Length) },
            { (Matrix32X32Txt, HeuristicKind.Manhattan), Heuristics.ResolveGridDimension(_files[Matrix32X32Txt].Length) },
            { (Matrix32X32Txt, HeuristicKind.Euclidean), Heuristics.ResolveGridDimension(_files[Matrix32X32Txt].Length) },
            { (Matrix64X64Txt, HeuristicKind.Manhattan), Heuristics.ResolveGridDimension(_files[Matrix64X64Txt].Length) },
            { (Matrix64X64Txt, HeuristicKind.Euclidean), Heuristics.ResolveGridDimension(_files[Matrix64X64Txt].Length) },
        };
    }
}