using System.Globalization;
using A3.RouteSearchGraphs.Attributes;
using Cocona;
using JetBrains.Annotations;

namespace A3.RouteSearchGraphs.Commands;

[UsedImplicitly]
internal record RunParameters(
    [Option(name: "file")] string File,
    [Option(name: "origin")] int Origin,
    [Option(name: "dest")] int Destination,
    [Option(name: "out")] string? OutputDirectory = null,
    [Option(name: "lang"), AllowedLanguage]
    string? Language = "pt-BR"
) : ICommandParameterSet
{
    internal CultureInfo GetCultureOrDefault()
    {
        return CultureInfo.GetCultureInfo(Language ?? "pt-BR");
    }
}