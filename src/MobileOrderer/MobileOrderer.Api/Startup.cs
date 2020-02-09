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
            services.AddHealthChecks();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            var eventBusConfig = new EventBusConfig();
            Configuration.GetSection("EventBusConfig").Bind(eventBusConfig);
            var credentials = new BasicAWSCredentials(eventBusConfig.AccessKey, eventBusConfig.SecretKey);

            services.Configure<Config>(options => Configuration.GetSection("Config").Bind(options));
            services.Configure<EventBusConfig>(options => Configuration.GetSection("EventBusConfig").Bind(options));

            // Shared
            services.AddDbContext<MobilesContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MobileDatabase")), ServiceLifetime.Singleton);
            services.AddSingleton<IRepository<Mobile>, MobileRepository>();
            services.AddSingleton<IEnumConverter, EnumConverter>();

            // API
            services.AddScoped<IGuidCreator, GuidCreator>();

            // HostedService
            services.AddSingleton<IHostedService, EventsService>();
            services.AddSingleton<IHostedService, EventPublisherService>();
            services.AddSingleton<IMobileRequestedEventChecker, MobileRequestedEventChecker>();
            services.AddSingleton<IMessagePublisher, MessagePublisher>();
            services.AddSingleton<IGetNewMobilesQuery, GetNewMobilesQuery>();
            services.AddSingleton<IMessageBusListenerBuilder, MessageBusListenerBuilder>();
            services.AddSingleton<ISqsService, SqsService>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IGetMobileByOrderIdQuery, GetMobileByOrderIdQuery>();
            services.AddSingleton<IQueueNamingStrategy, DefaultQueueNamingStrategy>();
            services.AddSingleton<ISnsService, SnsService>();
            services.AddSingleton<IMessageDeserializer, MessageDeserializer>();
            services.AddSingleton<AWSCredentials>(credentials);
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