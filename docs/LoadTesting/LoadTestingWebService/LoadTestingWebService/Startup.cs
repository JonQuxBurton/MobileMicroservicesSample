using System.Collections.Generic;
using LoadTestingWebService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LoadTestingWebService
{
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
            services.Configure<TestDataSettings>(options => Configuration.GetSection("TestDataSettings").Bind(options));
            services.Configure<ScenariosSettings>(options => Configuration.GetSection("ScenariosSettings").Bind(options));

            services.AddControllers();

            services.AddSingleton<IScenarioLogger, ScenarioLogger>();
            services.AddSingleton<IScenariosService, ScenariosService>();
            services.AddSingleton<IScenarioScriptFileWriter, ScenarioScriptFileWriter>();
            services.AddSingleton<IDataStore, DataStore>();
            services.AddSingleton<IDataGenerator, DataGenerator>();
            services.AddSingleton<IDataFileWriter, DataFileWriter>();
            services.AddSingleton<IDataStoreWriter, DataStoreWriter>();
            services.AddSingleton<IScenariosDataBuilder, ScenariosDataBuilder>();
            services.AddSingleton<IScenariosFactory, ScenariosFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IScenariosService scenariosService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            scenariosService.GenerateData();
        }
    }
}
