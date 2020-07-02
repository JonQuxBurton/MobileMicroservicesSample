using FluentAssertions;
using System.Net;
using System.Net.Http;
using Xunit;
using System.Text.Json;

namespace EndToEndApiLevelTests
{
    public class Scenario_Order_A_Mobile
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

            actualResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var mobileGlobalId = actualMobileReturned.GlobalId;

            var actualMobile = mobilesData.GetMobile(mobileGlobalId);
            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobile.Id);

            var actualSimCardOrder = simCardsData.TryGetSimCardOrder(actualMobileOrder.GlobalId);
            var actualExternalSimCardOrder = externalSimCardOrdersData.TryGetExternalSimCardOrder(actualMobileOrder.GlobalId);

            // Check Mobiles database
            actualMobile.Should().NotBeNull();
            actualMobileOrder.Should().NotBeNull();
            actualMobileOrder.Name.Should().Be(expectedOrder.Name);
            actualMobileOrder.ContactPhoneNumber.Should().Be(expectedOrder.ContactPhoneNumber);

            // Check SIM Cards database
            actualSimCardOrder.Should().NotBeNull();
            actualSimCardOrder.Name.Should().Be(expectedOrder.Name);

            // Check External SIM Card system
            actualExternalSimCardOrder.Should().NotBeNull();
        }
    }
}
