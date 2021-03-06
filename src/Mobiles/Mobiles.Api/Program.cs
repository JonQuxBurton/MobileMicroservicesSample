﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Mobiles.Api.Configuration;
using Serilog;
using Serilog.Formatting.Json;

namespace Mobiles.Api
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
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(new JsonFormatter(), logFilePath, shared: true)
                .WriteTo.Seq(config.SeqUrl)
                .CreateLogger();

            try
            {
                Console.Title = $"{programName }";

                Log.ForContext<Program>().Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
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

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
    }
}
