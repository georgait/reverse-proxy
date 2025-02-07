// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Threading.Tasks;

namespace Yarp.Kubernetes.Controller;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var serilog = new LoggerConfiguration()
           .MinimumLevel.Debug()
           .Enrich.FromLogContext()
           .WriteTo.Console(theme: AnsiConsoleTheme.Code)
           .CreateLogger();

        ServiceClientTracing.IsEnabled = true;

        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("/app/config/yarp.json", optional: true);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(serilog, dispose: false);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .Build()
            .RunAsync().ConfigureAwait(false);
    }
}
