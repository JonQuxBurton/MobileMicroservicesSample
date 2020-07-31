using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MinimalEventBus;
using SimCards.EventHandlers.Data;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Runtime;
using MinimalEventBus.Aws;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Domain;
using SimCards.EventHandlers.BackgroundServices;
using Serilog;
using Serilog.Formatting.Json;

namespace SimCards.EventHandlers
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        public static int Main(string[] args)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var programName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            var config = new Config();
            Configuration.GetSection("Config").Bind(config);
            var logFilePath = $"{config.LogFilePath}{programName }.json";

            Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(new JsonFormatter(), logFilePath, shared: true)
            .CreateLogger();

            try
            {
                Log.Information("Starting host");
                Console.Title = $"{programName}";
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

        private static IConfiguration Configuration;

        private static void ConfigureServices(IServiceCollection services)
        {
            var eventBusConfig = new EventBusConfig();
            Configuration.GetSection("EventBusConfig").Bind(eventBusConfig);
            var credentials = new BasicAWSCredentials(eventBusConfig.AccessKey, eventBusConfig.SecretKey);

            services.Configure<Config>(options => Configuration.GetSection("Config").Bind(options));
            services.Configure<EventBusConfig>(options => Configuration.GetSection("EventBusConfig").Bind(options));
            services.AddHttpClient<IExternalSimCardsProviderService, ExternalSimCardsProviderService>();

            services.AddSingleton<AWSCredentials>(credentials);
            services.AddSingleton<ISimCardOrdersDataStore, SimCardOrdersDataStore>();
            services.AddSingleton<ISqsService, SqsService>();
            services.AddSingleton<ISnsService, SnsService>();
            services.AddSingleton<IQueueNamingStrategy, DefaultQueueNamingStrategy>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IMessageBusListenerBuilder, MessageBusListenerBuilder>();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<ICompletedOrderChecker, CompletedOrderChecker>();
            services.AddSingleton<IMonitoring, Monitoring>();

            services.AddHostedService<MetricsServerHost>();
            services.AddHostedService<EventListenerHostedService>();
            services.AddHostedService<CompletedOrderPollingHostedService>();
        }

        public static IHost BuildHost(string[] args) =>
            new HostBuilder()
                .ConfigureServices(services => ConfigureServices(services))
                .UseSerilog()
                .Build();
    }
}
