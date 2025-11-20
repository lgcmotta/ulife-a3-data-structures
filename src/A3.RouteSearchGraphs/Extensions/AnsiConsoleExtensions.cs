using System.Globalization;
using A3.RouteSearchGraphs.Domain.Abstractions;
using Microsoft.Extensions.Localization;
using Spectre.Console;

namespace A3.RouteSearchGraphs.Extensions;

internal static class AnsiConsoleExtensions
{
    extension(IAnsiConsole console)
    {
        internal void WriteAlgorithmExecution(IStringLocalizer<SharedResource> localizer, SearchAlgorithmResult result)
        {
            var template = localizer["execution"].Value;

            var algorithm = string.IsNullOrWhiteSpace(result.Heuristic)
                ? result.Name
                : $"{result.Name} {localizer["with_heuristic"].Value} {result.Heuristic}";

            var message = string.Format(template, algorithm, result.ExecutionTime.TotalMilliseconds.ToString("F4", CultureInfo.InvariantCulture));

            console.MarkupLine($"[green]{message}[/]");
        }

        internal void WriteReportWritten(IStringLocalizer<SharedResource> localizer, string report)
        {
            console.MarkupLineInterpolated($"[yellow]{localizer["report_written"].Value}[/]: {report}");
        }

        internal void WriteError(IStringLocalizer<SharedResource> localizer, Exception exception)
        {
            console.MarkupLineInterpolated($"[red]{localizer["error"].Value}[/]: {exception.Message}");
        }
    }
}