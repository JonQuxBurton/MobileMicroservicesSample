using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.BackgroundServices;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog;
using Serilog.Formatting.Json;
using Utils.Enums;

namespace MobileTelecomsNetwork.EventHandlers
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
                .WriteTo.Seq(config.SeqUrl)
                .CreateLogger();

            try
            {
                Log.ForContext<Program>().Information("Starting host");
                Console.Title = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}";
                BuildHost().Run();
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

        private static IConfiguration Configuration;

        private static void ConfigureServices(IServiceCollection services)
        {
            var eventBusConfig = new EventBusConfig();
            Configuration.GetSection("EventBusConfig").Bind(eventBusConfig);
            var credentials = new BasicAWSCredentials(eventBusConfig.AccessKey, eventBusConfig.SecretKey);
            
            services.Configure<EventBusConfig>(options => Configuration.GetSection("EventBusConfig").Bind(options));
            services.Configure<Config>(options => Configuration.GetSection("Config").Bind(options));
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
            services.AddSingleton<IEnumConverter, EnumConverter>();

            services.AddHostedService<MetricsServerHost>();
            services.AddHostedService<EventListenerHostedService>();
            services.AddHostedService<CompletedOrderPollingHostedService>();
        }

        public static IHost BuildHost() =>
            new HostBuilder()
                .ConfigureServices(services => ConfigureServices(services))
                .UseSerilog()
                .Build();
    }
}
