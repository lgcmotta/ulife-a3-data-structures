using A3.RouteSearchGraphs.Categories;
using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Algorithms;
using BenchmarkDotNet.Attributes;

namespace A3.RouteSearchGraphs.Benchmarks;

public partial class GraphSearchBenchmarks
{
    [Benchmark(Description = "Breadth First Search"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> BreadthFirstSearch16X16() =>
        await new BreadthFirstSearch().Execute(_files[Matrix16X16Txt], 0, 255);

    [Benchmark(Description = "Depth First Search"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> DepthFirstSearch16X16() =>
        await new DepthFirstSearch().Execute(_files[Matrix16X16Txt], 0, 255);

    [Benchmark(Description = "Dijkstra"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> DijkstraSearch16X16() =>
        await new DijkstraSearch().Execute(_files[Matrix16X16Txt], 0, 255);

    [Benchmark(Description = "Greedy Best First Search - Manhattan"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchManhattan16X16() =>
        await new GreedyBestFirstSearch(HeuristicKind.Manhattan, _dimensions[(Matrix16X16Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix16X16Txt], 0, 255);

    [Benchmark(Description = "Greedy Best First Search - Euclidean"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> GreedyBestFirstSearchEuclidean16X16() =>
        await new GreedyBestFirstSearch(HeuristicKind.Euclidean, _dimensions[(Matrix16X16Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix16X16Txt], 0, 255);

    [Benchmark(Description = "A* - Manhattan"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchManhattan16X16() =>
        await new AStarSearch(HeuristicKind.Manhattan, _dimensions[(Matrix16X16Txt, HeuristicKind.Manhattan)]).Execute(_files[Matrix16X16Txt], 0, 255);

    [Benchmark(Description = "A* - Euclidean"), BenchmarkCategory(SizeCategory.Category16X16)]
    public async ValueTask<SearchAlgorithmResult> AStarSearchEuclidean16X16() =>
        await new AStarSearch(HeuristicKind.Euclidean, _dimensions[(Matrix16X16Txt, HeuristicKind.Euclidean)]).Execute(_files[Matrix16X16Txt], 0, 255);
}