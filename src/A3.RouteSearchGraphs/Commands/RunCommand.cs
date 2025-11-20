using A3.RouteSearchGraphs.Domain.Abstractions;
using A3.RouteSearchGraphs.Domain.Algorithms;
using A3.RouteSearchGraphs.Domain.Extensions;
using A3.RouteSearchGraphs.Extensions;
using A3.RouteSearchGraphs.Reports;
using Cocona;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace A3.RouteSearchGraphs.Commands;

internal static class RunCommand
{
    extension(CoconaApp app)
    {
        internal CoconaApp AddRunCommand()
        {
            app.AddCommand(name: "run", ExecuteRunCommandAsync);

            return app;
        }

        private static async ValueTask<int> ExecuteRunCommandAsync(
            RunParameters parameters,
            IAnsiConsole console,
            IStringLocalizer<SharedResource> localizer,
            CoconaAppContext context)
        {
            var token = context.CancellationToken;

            var (ui, current) = Thread.CurrentThread.GetCurrentCultures();

            var culture = parameters.GetCultureOrDefault();

            Thread.CurrentThread.SetCurrentCultures(culture);

            try
            {
                var input = Path.GetFullPath(parameters.File);

                var matrix = await input.ParseAdjacencyMatrix(token);

                matrix.EnsureValidIndex(parameters.Origin, nameof(parameters.Origin));
                matrix.EnsureValidIndex(parameters.Destination, nameof(parameters.Destination));

                var dimension = Heuristics.ResolveGridDimension(matrix.Length);

                var algorithms = BuildAlgorithms(dimension);

                List<SearchAlgorithmResult> results = [];

                foreach (var algorithm in algorithms)
                {
                    token.ThrowIfCancellationRequested();

                    var result = await algorithm.Execute(matrix, parameters.Origin, parameters.Destination, token);

                    results.Add(result);

                    console.WriteAlgorithmExecution(localizer, result);
                }

                var output = parameters.OutputDirectory ?? Defaults.DefaultOutputDirectory;

                var writer = new ReportWriter(localizer);

                foreach (var result in results)
                {
                    token.ThrowIfCancellationRequested();

                    var reportPath = await writer.WriteAsync(result, parameters.File, output, token);

                    console.WriteReportWritten(localizer, reportPath);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                console.WriteError(localizer, exception);

                return 1;
            }
            finally
            {
                Thread.CurrentThread.SetCurrentCultures(ui, current);
            }

            return 0;
        }

        private static IReadOnlyCollection<IGraphSearchAlgorithm> BuildAlgorithms(int gridDimension) =>
        [
            new BreadthFirstSearch(),
            new DepthFirstSearch(),
            new DijkstraSearch(),
            new GreedyBestFirstSearch(HeuristicKind.Manhattan, gridDimension),
            new GreedyBestFirstSearch(HeuristicKind.Euclidean, gridDimension),
            new AStarSearch(HeuristicKind.Manhattan, gridDimension),
            new AStarSearch(HeuristicKind.Euclidean, gridDimension)
        ];
    }
}