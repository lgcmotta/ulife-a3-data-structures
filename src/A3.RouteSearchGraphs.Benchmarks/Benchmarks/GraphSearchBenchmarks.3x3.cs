using A3.RouteSearchGraphs.Categories;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Algorithms;
using BenchmarkDotNet.Attributes;

namespace A3.RouteSearchGraphs.Benchmarks;

public partial class GraphSearchBenchmarks
{
    [Benchmark(Description = "Breadth First Search"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> BreadthFirstSearch3X3() =>
        await new BreadthFirstSearch().Execute(_files[Matrix3X3Txt], 0, 8);

    [Benchmark(Description = "Depth First Search"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> DepthFirstSearch3X3() =>
        await new DepthFirstSearch().Execute(_files[Matrix3X3Txt], 0, 8);

    [Benchmark(Description = "Dijkstra"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> DijkstraSearch3X3() =>
        await new DijkstraSearch().Execute(_files[Matrix3X3Txt], 0, 8);

    [Benchmark(Description = "Greedy Best First Search - Manhattan"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchManhattan3X3() =>
        await new GreedyBestFirstSearch(HeuristicKind.Manhattan, _dimensions[(Matrix3X3Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix3X3Txt], 0, 8);

    [Benchmark(Description = "Greedy Best First Search - Euclidean"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchEuclidean3X3() =>
        await new GreedyBestFirstSearch(HeuristicKind.Euclidean, _dimensions[(Matrix3X3Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix3X3Txt], 0, 8);

    [Benchmark(Description = "A* - Manhattan"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchManhattan3X3() =>
        await new AStarSearch(HeuristicKind.Manhattan, _dimensions[(Matrix3X3Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix3X3Txt], 0, 8);

    [Benchmark(Description = "A* - Euclidean"), BenchmarkCategory(SizeCategory.Category3X3)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchEuclidean3X3() =>
        await new AStarSearch(HeuristicKind.Euclidean, _dimensions[(Matrix3X3Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix3X3Txt], 0, 8);
}