using System.Globalization;
using A3.RouteSearchGraphs.Attributes;
using Cocona;
using JetBrains.Annotations;

namespace A3.RouteSearchGraphs.Commands;

[UsedImplicitly]
internal record RunParameters(
    [Option(name: "file", shortNames: ['f'], Description = "file", ValueName = "string")]
    string File,
    [Option(name: "origin", shortNames: ['s'], Description = "origin", ValueName = "int")]
    int Origin,
    [Option(name: "target", shortNames: ['t'], Description = "target", ValueName = "int")]
    int Destination,
    [Option(name: "out", shortNames: ['o'], Description = "out", ValueName = "string")]
    string? OutputDirectory = null,
    [Option(name: "lang", shortNames: ['l'], Description = "lang", ValueName = "pt-BR / en-US"), AllowedLanguage]
    string? Language = "pt-BR"
) : ICommandParameterSet
{
    internal CultureInfo GetCultureOrDefault()
    {
        return CultureInfo.GetCultureInfo(Language ?? "pt-BR");
    }
}