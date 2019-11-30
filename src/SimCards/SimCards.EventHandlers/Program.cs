﻿using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MinimalEventBus;
using SimCards.EventHandlers.Data;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Runtime;
using MinimalEventBus.Aws;
using SimCards.EventHandlers.Services;
using System.Diagnostics.CodeAnalysis;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Hosting;

namespace SimCards.EventHandlers
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        private static void ConfigureServices(IServiceCollection services)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile($"appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                        .AddEnvironmentVariables();
            var configuration = builder.Build();

            var eventBusConfig = new EventBusConfig();
            configuration.GetSection("EventBusConfig").Bind(eventBusConfig);
            var credentials = new BasicAWSCredentials(eventBusConfig.AccessKey, eventBusConfig.SecretKey);

            services.Configure<Config>(options => configuration.GetSection("Config").Bind(options));
            services.Configure<EventBusConfig>(options => configuration.GetSection("EventBusConfig").Bind(options));
            services.AddHttpClient<ISimCardWholesaleService, SimCardWholesaleService>();

            services.AddSingleton<AWSCredentials>(credentials);
            services.AddTransient<ISimCardOrdersDataStore, SimCardOrdersDataStore>();
            services.AddTransient<ISqsService, SqsService>();
            services.AddTransient<ISnsService, SnsService>();
            services.AddTransient<IQueueNamingStrategy, DefaultQueueNamingStrategy>();
            services.AddTransient<IMessageBus, MessageBus>();
            services.AddTransient<IMessageBusListenerBuilder, MessageBusListenerBuilder>();

            services.AddSingleton<IHostedService, HostedService>();
        }

        public static IHost BuildHost(string[] args) =>
        new HostBuilder()
            .ConfigureServices(services => ConfigureServices(services))
            .UseSerilog()
            .Build();

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            try
            {
                Log.Information("Starting host");
                Console.Title = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}";
                BuildHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}