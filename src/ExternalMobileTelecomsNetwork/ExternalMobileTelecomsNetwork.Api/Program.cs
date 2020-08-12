using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ExternalMobileTelecomsNetwork.Api.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ExternalMobileTelecomsNetwork.Api
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            var programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            var config = new Config();
            Configuration.GetSection("Config").Bind(config);
            var logFilePath = $"{config.LogFilePath}{programName }.json";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.Seq(config.SeqUrl)
                    .CreateLogger();

            try
            {
                Console.Title = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}";
                Log.ForContext<Program>().Information("Starting web host");

                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>().Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}
