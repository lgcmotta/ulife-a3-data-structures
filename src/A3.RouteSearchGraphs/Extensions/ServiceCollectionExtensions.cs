using A3.RouteSearchGraphs.Translations;
using Cocona.Localization;
using Spectre.Console;

namespace A3.RouteSearchGraphs.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddAnsiConsole()
        {
            return services.AddSingleton<IAnsiConsole>(_ => AnsiConsole.Console);
        }

        internal IServiceCollection AddCommandsLocalization()
        {
            return services.AddTransient<ICoconaLocalizer, CoconaLocalizer>();
        }

        internal IServiceCollection AddReportLocalization()
        {
            return services.AddLocalization(options => options.ResourcesPath = "Resources");
        }
    }
}