using Cocona.Command;
using Cocona.Localization;
using Microsoft.Extensions.Localization;

namespace A3.RouteSearchGraphs.Translations;

internal class CoconaLocalizer(IStringLocalizerFactory factory) : ICoconaLocalizer
{
    private readonly IStringLocalizer _localizer = factory.Create(typeof(Program));

    public string GetCommandDescription(CommandDescriptor command)
    {
        return _localizer.GetString(command.Name is "ShowHelp" ? "help" : command.Description);
    }

    public string GetOptionDescription(CommandDescriptor command, ICommandOptionDescriptor option)
    {
        return _localizer.GetString(option.Description);
    }

    public string GetArgumentDescription(CommandDescriptor command, CommandArgumentDescriptor argument)
    {
        return _localizer.GetString(argument.Description);
    }
}