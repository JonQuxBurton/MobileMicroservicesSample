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
using Mobiles.Api.Configuration;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Services;
using Prometheus;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Enums;
using Utils.Guids;

namespace Mobiles.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            // Utilities
            services.AddSingleton<IDateTimeCreator, DateTimeCreator>();
            services.AddSingleton<IEnumConverter, EnumConverter>();
            services.AddSingleton<IMessageDeserializer, MessageDeserializer>();
            services.AddSingleton<AWSCredentials>(credentials);

            // API
            services.AddScoped<IGuidCreator, GuidCreator>();
            services.AddScoped<IMessagePublisher, MessagePublisher>();
            services.AddScoped<IGetNewProvisionsQuery, GetNewProvisionsQuery>();
            services.AddScoped<IGetProcessingProvisionMobilesQuery, GetProcessingProvisionMobilesQuery>();
            services.AddScoped<IGetNewActivatesQuery, GetNewActivatesQuery>();
            services.AddScoped<IGetNewCeasesQuery, GetNewCeasesQuery>();
            services.AddScoped<IGetMobileByOrderIdQuery, GetMobileByOrderIdQuery>();
            services.AddScoped<IGetMobilesByCustomerIdQuery, GetMobilesByCustomerIdQuery>();
            services.AddScoped<IGetNextMobileIdQuery, GetNextMobileIdQuery>();
            services.AddScoped<ICustomersService, CustomersService>();
            services.AddScoped<IMobilesService, MobilesService>();
            
            // EventBus
            services.AddSingleton<ISnsService, SnsService>();
            services.AddSingleton<ISqsService, SqsService>();
            services.AddSingleton<IMessageBus, MessageBus>();
            services.AddSingleton<IMessageBusListenerBuilder, MessageBusListenerBuilder>();
            services.AddSingleton<IQueueNamingStrategy, DefaultQueueNamingStrategy>();

            services.AddSingleton<IMonitoring>(new Monitoring());

            services.AddScoped<IMobileEventsChecker, MobileProvisionRequestedEventChecker>();
            services.AddScoped<IMobileEventsChecker, ProcessingProvisionEventChecker>();
            services.AddScoped<IMobileEventsChecker, ActivateRequestedEventChecker>();
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

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHealthChecks("/health");
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.UseSerilogRequestLogging();
            app.UseMvc();

            var eventBusConfig = new EventBusConfig();
            Configuration.GetSection("EventBusConfig").Bind(eventBusConfig);

            logger.LogDebug($"SnsServiceUrl: {eventBusConfig.SnsServiceUrl}");
            logger.LogDebug($"SqsServiceUrl: {eventBusConfig.SqsServiceUrl}");
        }
    }
}