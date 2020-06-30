using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using MinimalEventBus;
using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Messages;
using MobileOrderer.Api.Resources;
using MobileOrderer.Api.Services;
using Newtonsoft.Json;
using SimCards.EventHandlers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Utils.Guids;
using Xunit;

namespace IntegrationTests
{
    public class Scenario_OrderAMobileFixture : IDisposable
    {
        public HttpResponseMessage provisionOrderResponse;
        public MobilesContext mobilesDatabaseFake;
        public Scenario_OrderAMobileFixture fixture;
        public InMemoryMessagePublisher messagePublisherFake;
        public MobileEventCheckersRunner mobileEventCheckersRunner;

        public OrderToAdd orderToAdd;

        private CustomWebApplicationFactory<MobileOrderer.Api.Startup> factory;

        public Scenario_OrderAMobileFixture()
        {
            this.factory = new CustomWebApplicationFactory<MobileOrderer.Api.Startup>();
            ExecuteScenario();
        }

        private void ExecuteScenario()
        {
            // Create MobileOrderer Service
            messagePublisherFake = new InMemoryMessagePublisher();
            void ConfigureTestServices(IServiceCollection services)
            {
                services.AddSingleton<IMessagePublisher>(messagePublisherFake);

                // Disable HostedServices
                //var descriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(EventPublisherService));

                RemoveHostedService<EventPublisherService>(services);
            }

            var webHostBuilder = factory
                .WithWebHostBuilder(builder =>
                    builder.ConfigureTestServices(ConfigureTestServices));

            var client = webHostBuilder.CreateClient();

            // Get HostedService from original 
            //var descriptor = webHostBuilder.Services.SingleOrDefault(d => d.ImplementationType == typeof(EventPublisherService));
            // webHostBuilder.Services.GetService<>(descriptor)
            //var aaa  = webHostBuilder.Services.GetService(typeof(EventPublisherService));
            //var eventPublisherService = webHostBuilder.Services.GetService(typeof(IHostedService)) as EventPublisherService;
            //eventPublisherService.StopAsync(new CancellationToken());

            mobileEventCheckersRunner = webHostBuilder.Services.GetService(typeof(IMobileEventCheckersRunner)) as MobileEventCheckersRunner;

            //var factory = new CustomWebApplicationFactory<Startup>();
            //var client = factory.WithWebHostBuilder(builder =>
            //{
            //    builder.ConfigureTestServices(services =>
            //    {
            //        //var messagePublisherDescriptor = services.SingleOrDefault(
            //        //        d => d.ServiceType == typeof(IMessagePublisher));

            //        //if (messagePublisherDescriptor != null)
            //        //    services.Remove(messagePublisherDescriptor);

            //        //services.AddSingleton<IMessagePublisher, InMemoryMessagePublisher>();
            //    });
            //})
            //    .CreateClient();
            //var client = fixture.CreateClient();
            var url = "/api/provisioner";

            // Get the InMemory DB
            mobilesDatabaseFake = webHostBuilder.Services.GetService(typeof(MobilesContext)) as MobilesContext;

            // 1. Post Order to Mobileorderer Ms
            // Provision Order
            orderToAdd = new OrderToAdd
            {
                Name = "Neil"

            };
            var request = new StringContent(JsonConvert.SerializeObject(orderToAdd));
            request.Headers.Clear();
            request.Headers.Add("content-type", "application/json");

            provisionOrderResponse = client.PostAsync(url, request).Result;

            mobileEventCheckersRunner.Check();

            // 2. Publish MobileRequestedMessage

            // 3.  SimCard EventHandler handles MobileRequestedMessage
            // Assert Order was added to the SimCards DB

            var hostBuilder = SimCards.EventHandlers.Program.GetHostBuilder();
            hostBuilder.ConfigureServices(services => {
                //var descriptor = services.SingleOrDefault(
                //    d => d.ServiceType ==
                //        typeof(DbContextOptions<MobilesContext>));

                //if (descriptor != null)
                //{
                //    services.Remove(descriptor);
                //}

                //// Add ApplicationDbContext using an in-memory database for testing.
                //services.AddDbContext<MobilesContext>(options =>
                //{
                //    options.UseInMemoryDatabase("InMemoryDbForTesting");
                //}, ServiceLifetime.Singleton);

                //services.Configure<SimCards.EventHandlers.Config>(options => configuration.GetSection("Config").Bind(options));

                services.AddSingleton<ISimCardOrdersDataStore, InMemorySimCardOrdersDataStore>();
            });

            var simCardsService = SimCards.EventHandlers.Program.BuildHost(new string[0]);
            simCardsService.Run();


            // TODO - set up servies in SimCards service, and verify
        }

        private static void RemoveHostedService<T>(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ImplementationType == typeof(T));
            services.Remove(descriptor);
        }

        public void Dispose() { }
    }

    public class Scenario_OrderAMobile 
        : 
        //IClassFixture<CustomWebApplicationFactory<MobileOrderer.Api.Startup>>
        IClassFixture<Scenario_OrderAMobileFixture>
    {
        private Scenario_OrderAMobileFixture scenario;

        public Scenario_OrderAMobile(Scenario_OrderAMobileFixture fixture)
        {
            this.scenario = fixture;
        }

        // TODO
        // Can I split into seperates, but they all share the setup only happens once
        // (and includes the test run, then the test can just be the assert of each step)
        // https://xunit.net/docs/shared-context

        [Fact]
        public void Step1_PostOrder__ReturnsOk()
        {
            scenario.provisionOrderResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async void Step1_PostOrder_AddsOrderToMobilesDatabase()
        {
            scenario.mobilesDatabaseFake.Mobiles.Count().Should().Be(1);
            var mobileOrderInDb = scenario.mobilesDatabaseFake.Mobiles.First().Orders.First();
            mobileOrderInDb.Name.Should().Be(scenario.orderToAdd.Name);
            mobileOrderInDb.ContactPhoneNumber.Should().Be(scenario.orderToAdd.ContactPhoneNumber);
        }

        [Fact]
        public async void Step2_PublishMobileRequestedMessage()
        {
            scenario.messagePublisherFake.Published.Count.Should().Be(1);
            var message = scenario.messagePublisherFake.Published.First() as MobileRequestedMessage;
            message.Name.Should().Be(scenario.orderToAdd.Name);
            message.ContactPhoneNumber.Should().Be(scenario.orderToAdd.ContactPhoneNumber);

            var mobileOrderInDb = scenario.mobilesDatabaseFake.Mobiles.First().Orders.First();
            message.MobileOrderId.Should().Be(mobileOrderInDb.GlobalId);
        }
    }
}
