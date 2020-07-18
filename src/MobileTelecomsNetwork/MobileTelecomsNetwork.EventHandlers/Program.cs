using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using MobileTelecomsNetwork.EventHandlers.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MobileTelecomsNetwork.EventHandlers
{
    [ExcludeFromCodeCoverage]
    class Program
    {
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

        public static IHost BuildHost(string[] args) =>
            new HostBuilder()
                .ConfigureServices(services => ConfigureServices(services))
                .UseSerilog()
                .Build();

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
            services.Configure<EventBusConfig>(options => configuration.GetSection("EventBusConfig").Bind(options));
            services.Configure<Config>(options => configuration.GetSection("Config").Bind(options));
            services.AddHttpClient<IExternalMobileTelecomsNetworkService, ExternalMobileTelecomsNetworkService>();

            services.AddSingleton<AWSCredentials>(credentials);
            services.AddSingleton<ISqsService, SqsService>();
            services.AddSingleton<ISnsService, SnsService>();
            services.AddSingleton<IQueueNamingStrategy, DefaultQueueNamingStrategy>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IMessageBusListenerBuilder, MessageBusListenerBuilder>();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<IDataStore, DataStore>();
            services.AddSingleton<IOrderCompletedChecker, OrderCompletedChecker>();
            services.AddSingleton<IMonitoring, Monitoring>();

            services.AddHostedService<MetricsServerHost>();
            services.AddHostedService<EventListenerHostedService>();
            services.AddHostedService<CompletedOrderPollingHostedService>();
        }
    }
}
