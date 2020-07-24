using Amazon.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MinimalEventBus;
using MinimalEventBus.Aws;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Services;
using Prometheus;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Utils.DomainDrivenDesign;
using Utils.Enums;
using Utils.Guids;

namespace MobileOrderer.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);

            var eventBusConfig = new EventBusConfig();
            Configuration.GetSection("EventBusConfig").Bind(eventBusConfig);
            var credentials = new BasicAWSCredentials(eventBusConfig.AccessKey, eventBusConfig.SecretKey);

            services.Configure<Config>(options => Configuration.GetSection("Config").Bind(options));
            services.Configure<EventBusConfig>(options => Configuration.GetSection("EventBusConfig").Bind(options));

            // Shared
            var config = new Config();
            Configuration.GetSection("Config").Bind(config);
            
            services.AddDbContext<MobilesContext>(options =>
                options.UseSqlServer(config.ConnectionString));
            services.AddScoped<IRepository<Mobile>, MobileRepository>();

            // Utilities
            services.AddSingleton<IEnumConverter, EnumConverter>();
            services.AddSingleton<IMessageDeserializer, MessageDeserializer>();
            services.AddSingleton<AWSCredentials>(credentials);

            // API
            services.AddScoped<IGuidCreator, GuidCreator>();
            services.AddScoped<IMessagePublisher, MessagePublisher>();
            services.AddScoped<IGetNewMobilesQuery, GetNewMobilesQuery>();
            services.AddScoped<IGetProcessingProvisioningMobilesQuery, GetProcessingProvisioningMobilesQuery>();
            services.AddScoped<IGetNewActivationsQuery, GetNewActivationsQuery>();
            services.AddScoped<IGetNewCeasesQuery, GetNewCeasesQuery>();
            services.AddScoped<IGetMobileByOrderIdQuery, GetMobileByOrderIdQuery>();
            
            // EventBus
            services.AddSingleton<ISnsService, SnsService>();
            services.AddSingleton<ISqsService, SqsService>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IMessageBusListenerBuilder, MessageBusListenerBuilder>();
            services.AddSingleton<IQueueNamingStrategy, DefaultQueueNamingStrategy>();

            services.AddSingleton<IMonitoring>(new Monitoring());

            services.AddScoped<IMobileEventsChecker, NewMobileEventChecker>();
            services.AddScoped<IMobileEventsChecker, ProcessingProvisioningEventChecker>();
            services.AddScoped<IMobileEventsChecker, ActivationRequestedEventChecker>();
            services.AddScoped<IMobileEventsChecker, CeaseRequestedEventChecker>();

            // HostedService
            services.AddHostedService<EventsService>();
            services.AddHostedService<EventPublisherService>();

            services.AddHealthChecks()
                .ForwardToPrometheus();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseMvc();

            app.UseHealthChecks("/health");

            app.UseMetricServer();
            app.UseHttpMetrics();

            var eventBusConfig = new EventBusConfig();
            Configuration.GetSection("EventBusConfig").Bind(eventBusConfig);

            logger.LogInformation($"SnsServiceUrl: {eventBusConfig.SnsServiceUrl}");
            logger.LogInformation($"SqsServiceUrl: {eventBusConfig.SqsServiceUrl}");

            var config = new Config();
            Configuration.GetSection("Config").Bind(config);
            logger.LogInformation($"ConnectionString: {config.ConnectionString}");
        }
    }
}