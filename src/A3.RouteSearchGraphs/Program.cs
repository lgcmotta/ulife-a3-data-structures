using System.Runtime.Loader;
using A3.RouteSearchGraphs.Commands;
using A3.RouteSearchGraphs.Extensions;
using Cocona;

CancellationTokenSource cts = new();

Console.CancelKeyPress += HandleCancelKeyPress;
AppDomain.CurrentDomain.ProcessExit += HandleCancelKeyPress;
AssemblyLoadContext.Default.Unloading += HandleUnloading;

var builder = CoconaApp.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Services.AddAnsiConsole();
builder.Services.AddReportLocalization();
builder.Services.AddCommandsLocalization();

var app = builder.Build();

app.AddRunCommand();

try
{
    await app.RunAsync();
}
catch (OperationCanceledException)
{
    // Application is shutting down with SIGTERM or SIGINT
}

return;

void HandleCancelKeyPress(object? source, EventArgs args)
{
    if (args is ConsoleCancelEventArgs eventArgs)
    {
        eventArgs.Cancel = true;
    }

    cts.Cancel();
}

void HandleUnloading(AssemblyLoadContext _)
{
    cts.Cancel();
}