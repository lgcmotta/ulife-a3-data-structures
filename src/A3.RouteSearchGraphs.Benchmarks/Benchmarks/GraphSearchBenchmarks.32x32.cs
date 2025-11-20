using A3.RouteSearchGraphs.Categories;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Algorithms;
using BenchmarkDotNet.Attributes;

namespace A3.RouteSearchGraphs.Benchmarks;

public partial class GraphSearchBenchmarks
{
    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> BreadthFirstSearch32X32() =>
        await new BreadthFirstSearch().Execute(_files[Matrix32X32Txt], 0, 1023);

    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> DepthFirstSearch32X32() =>
        await new DepthFirstSearch().Execute(_files[Matrix32X32Txt], 0, 1023);

    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> DijkstraSearch32X32() =>
        await new DijkstraSearch().Execute(_files[Matrix32X32Txt], 0, 1023);

    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchManhattan32X32() =>
        await new GreedyBestFirstSearch(HeuristicKind.Manhattan, _dimensions[(Matrix32X32Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix32X32Txt], 0, 1023);

    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchEuclidean32X32() =>
        await new GreedyBestFirstSearch(HeuristicKind.Euclidean, _dimensions[(Matrix32X32Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix32X32Txt], 0, 1023);

    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchManhattan32X32() =>
        await new AStarSearch(HeuristicKind.Manhattan, _dimensions[(Matrix32X32Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix32X32Txt], 0, 1023);

    [Benchmark, BenchmarkCategory(SizeCategory.Category32X32)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchEuclidean32X32() =>
        await new AStarSearch(HeuristicKind.Euclidean, _dimensions[(Matrix32X32Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix32X32Txt], 0, 1023);
}