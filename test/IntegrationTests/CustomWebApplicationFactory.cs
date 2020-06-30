using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using MobileOrderer.Api.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MobileOrderer.Api.Domain;
using MinimalEventBus.JustSaying;
using Microsoft.AspNetCore.TestHost;

namespace IntegrationTests
{

    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        //public MobilesContext inMemoryMobilesContext;
        public IMessagePublisher inMemoryMessagePublisher;

        //protected override TestServer CreateServer(IWebHostBuilder builder)
        //{
        //    var a = 1;



        //    return base.CreateServer(builder);
        //}

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //var messagePublisherDescriptor = services.SingleOrDefault(
                //    d => d.ServiceType == typeof(IMessagePublisher));

                //if (messagePublisherDescriptor != null)
                //    services.Remove(messagePublisherDescriptor);

                services.AddSingleton<IMessagePublisher, InMemoryMessagePublisher>();

                // Remove the DbContext registration.
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<MobilesContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<MobilesContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                }, ServiceLifetime.Singleton);

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<MobilesContext>();
                    //inMemoryMobilesContext = db;
                    //var logger = scopedServices
                    //    .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    //inMemoryMessagePublisher = scopedServices.GetRequiredService<IMessagePublisher>();

                    //try
                    //{
                    //    // Seed the database with test data.
                    //    Utilities.InitializeDbForTests(db);
                    //}
                    //catch (Exception ex)
                    //{
                    //    logger.LogError(ex, "An error occurred seeding the " +
                    //        "database with test messages. Error: {Message}", ex.Message);
                    //}
                }
            });
        }
    }
}
