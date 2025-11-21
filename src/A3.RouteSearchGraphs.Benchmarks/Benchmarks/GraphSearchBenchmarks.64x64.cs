using A3.RouteSearchGraphs.Categories;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Algorithms;
using BenchmarkDotNet.Attributes;

namespace A3.RouteSearchGraphs.Benchmarks;

public partial class GraphSearchBenchmarks
{
    [Benchmark(Description = "Breadth First Search"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> BreadthFirstSearch64X64() =>
        await new BreadthFirstSearch().Execute(_files[Matrix64X64Txt], 0, 4095);

    [Benchmark(Description = "Depth First Search"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> DepthFirstSearch64X64() =>
        await new DepthFirstSearch().Execute(_files[Matrix64X64Txt], 0, 4095);

    [Benchmark(Description = "Dijkstra"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> DijkstraSearch64X64() =>
        await new DijkstraSearch().Execute(_files[Matrix64X64Txt], 0, 4095);

    [Benchmark(Description = "Greedy Best First Search - Manhattan"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchManhattan64X64() =>
        await new GreedyBestFirstSearch(HeuristicKind.Manhattan, _dimensions[(Matrix64X64Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix64X64Txt], 0, 4095);

    [Benchmark(Description = "Greedy Best First Search - Euclidean"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchEuclidean64X64() =>
        await new GreedyBestFirstSearch(HeuristicKind.Euclidean, _dimensions[(Matrix64X64Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix64X64Txt], 0, 4095);

    [Benchmark(Description = "A* - Manhattan"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchManhattan64X64() =>
        await new AStarSearch(HeuristicKind.Manhattan, _dimensions[(Matrix64X64Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix64X64Txt], 0, 4095);

    [Benchmark(Description = "A* - Euclidean"), BenchmarkCategory(SizeCategory.Category64X64)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchEuclidean64X64() =>
        await new GreedyBestFirstSearch(HeuristicKind.Euclidean, _dimensions[(Matrix64X64Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix64X64Txt], 0, 4095);
}