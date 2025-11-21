using A3.RouteSearchGraphs.Categories;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Algorithms;
using BenchmarkDotNet.Attributes;

namespace A3.RouteSearchGraphs.Benchmarks;

public partial class GraphSearchBenchmarks
{
    [Benchmark(Description = "Breadth First Search"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> BreadthFirstSearch4X4() =>
        await new BreadthFirstSearch().Execute(_files[Matrix4X4Txt], 0, 15);

    [Benchmark(Description = "Depth First Search"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> DepthFirstSearch4X4() =>
        await new DepthFirstSearch().Execute(_files[Matrix4X4Txt], 0, 15);

    [Benchmark(Description = "Dijkstra"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> DijkstraSearch4X4() =>
        await new DijkstraSearch().Execute(_files[Matrix4X4Txt], 0, 15);

    [Benchmark(Description = "Greedy Best First Search - Manhattan"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchManhattan4X4() =>
        await new GreedyBestFirstSearch(HeuristicKind.Manhattan, _dimensions[(Matrix4X4Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix4X4Txt], 0, 15);

    [Benchmark(Description = "Greedy Best First Search - Euclidean"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchEuclidean4X4() =>
        await new GreedyBestFirstSearch(HeuristicKind.Euclidean, _dimensions[(Matrix4X4Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix4X4Txt], 0, 15);

    [Benchmark(Description = "A* - Manhattan"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchManhattan4X4() =>
        await new AStarSearch(HeuristicKind.Manhattan, _dimensions[(Matrix4X4Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix4X4Txt], 0, 15);

    [Benchmark(Description = "A* - Euclidean"), BenchmarkCategory(SizeCategory.Category4X4)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchEuclidean4X4() =>
        await new AStarSearch(HeuristicKind.Euclidean, _dimensions[(Matrix4X4Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix4X4Txt], 0, 15);
}