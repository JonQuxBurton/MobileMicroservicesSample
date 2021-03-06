﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ExternalSimCardsProvider.Api.Configuration;
using ExternalSimCardsProvider.Api.Data;
using System.Diagnostics.CodeAnalysis;
using ExternalSimCardsProvider.Api.Domain;

namespace ExternalSimCardsProvider.Api
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
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.Configure<Config>(options => Configuration.GetSection("Config").Bind(options));

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddScoped<IOrdersDataStore, OrdersDataStore>();
            services.AddScoped<IRandomCharCodesGenerator, RandomCharCodesGenerator>();
            services.AddScoped<IActivationCodeGenerator, ActivationCodeGenerator>();
            services.AddHttpClient<IExternalMobileTelecomsNetworkService, ExternalMobileTelecomsNetworkService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseSerilogRequestLogging();
            app.UseMvc();
            app.UseHealthChecks("/health");
        }
    }
}
