using FluentAssertions;
using System.Net;
using System.Net.Http;
using Xunit;
using System.Text.Json;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using System;

namespace EndToEndApiLevelTests
{
    public class Scenario_Mobile_Order_Completed
    {
        internal void CheckReady()
        {
            // Check databases
            // Check MobileOrderer API
            // Check SIM Card Service
            // Check External SIM Card API
            // Check goaws
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public async void Execute()
        {
            CheckReady();

            //var connectionString = "Server=localhost\\SQLEXPRESS;Database=Mobile;Trusted_Connection=True;";
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";

            var mobilesData = new MobilesData(connectionString);
            var simCardsData = new SimCardsData(connectionString);
            var externalSimCardOrdersData = new ExternalSimCardOrders(connectionString);

            // POST to Mobile Orderer Web Application
            var client = new HttpClient();
            var url = "http://localhost:5000/api/provisioner";

            var expectedOrder = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage actualResponse = await client.PostAsJsonAsync(url, expectedOrder);
            var stringResponse = await actualResponse.Content.ReadAsStringAsync();
            var actualMobileReturned = JsonSerializer.Deserialize<MobileOrderer.Api.Resources.MobileResource>(stringResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            Thread.Sleep(60 * 1000);

            var mobileGlobalId = actualMobileReturned.GlobalId;
            var currentMobile = mobilesData.GetMobile(mobileGlobalId);
            var currentMobileOrder = mobilesData.GetMobileOrder(currentMobile.Id);

            var externalSimCardWholesalerUrl = "http://localhost:5001/api/orders/complete";
            var orderToComplete = new SimCardWholesaler.Api.Resources.OrderToComplete
            {
                Reference = currentMobileOrder.GlobalId
            };

            HttpResponseMessage actualCompleteOrderResponse = await client.PostAsJsonAsync(externalSimCardWholesalerUrl, orderToComplete);

            actualCompleteOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            Thread.Sleep(60 * 1000);

            var actualSimCardOrder = simCardsData.TryGetSimCardOrder(currentMobileOrder.GlobalId);
            var actualMobile = mobilesData.GetMobile(mobileGlobalId);
            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobile.Id);

            // Check Order is Completed in the SIM Cards database
            actualSimCardOrder.Should().NotBeNull();
            actualSimCardOrder.Status.Should().Be("Completed");

            // Check Mobiles database
            actualMobile.Should().NotBeNull();
            actualMobile.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.WaitingForActivation));

            actualMobileOrder.Should().NotBeNull();
            actualMobileOrder.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Order.State), MobileOrderer.Api.Domain.Order.State.Completed));
        }
    }
}
