using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace EndToEndApiLevelTests
{
    public partial class ScenariosFixture : IDisposable
    {
        public ScenariosFixture()
        {
            Execute().Wait();
        }

        private async Task Execute()
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            var mobilesData = new MobilesData(connectionString);

            var snapshotFactory = new SnapshotFactory(connectionString, TimeSpan.FromSeconds(10));

            // Scneario 1 Order a Mobile
            var client = new HttpClient();
            var url = "http://localhost:5000/api/provisioner";

            var scenario1OrderToAdd = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage orderAMobileResponse = await client.PostAsJsonAsync(url, scenario1OrderToAdd);

            orderAMobileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Gather needed data
            var actualMobileReturned = await DeserializeResponse<MobileOrderer.Api.Resources.MobileResource>(orderAMobileResponse);
            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobileReturned.Id);
            var mobileGlobalId = actualMobileReturned.GlobalId;
            var orderAMobileOrderReference = actualMobileOrder.GlobalId;

            // Take Scenario 1 Snapshot
            Scenario1_Snapshot = snapshotFactory.Take_Scenario1_Snapshot(scenario1OrderToAdd, mobileGlobalId, orderAMobileOrderReference);

            // Scenario 2 Complete Mobile Order
            var externalSimCardWholesalerUrl = $"http://localhost:5001/api/orders/{orderAMobileOrderReference}complete";

            HttpResponseMessage actualCompleteOrderResponse = await client.PostAsync(externalSimCardWholesalerUrl, null);

            actualCompleteOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Scenario 2 Snapshot
            Scenario2_Snapshot = snapshotFactory.Take_Scenario2_Snapshot(mobileGlobalId, orderAMobileOrderReference);

            // Scenario 3 Activate a Mobile
            var activateTheMobileUrl = $"http://localhost:5000/api/mobiles/{mobileGlobalId}/activate";
            var activateTheMobileOrder = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage actualActivateTheMobileResponse = await client.PostAsJsonAsync(activateTheMobileUrl, activateTheMobileOrder);

            actualActivateTheMobileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var actualActivationOrderReturned = await DeserializeResponse<MobileOrderer.Api.Resources.OrderResource>(actualActivateTheMobileResponse);
            var activateAMobileOrderReference = actualActivationOrderReturned.GlobalId;

            // Take Scenario 3 Snapshot
            Scenario3_Snapshot = snapshotFactory.Take_Scenario3_Snapshot(mobileGlobalId, activateAMobileOrderReference);

            // Scenario 4 Complete Activate Order
            var externalMobileTelecomsNetworkUrl = $"http://localhost:5002/api/orders/{activateAMobileOrderReference}/complete";

            HttpResponseMessage actualCompleteActivateOrderResponse = await client.PostAsync(externalMobileTelecomsNetworkUrl, null);

            actualCompleteActivateOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Scenario 4 Snapshot
            Scenario4_Snapshot = snapshotFactory.Take_Scenario4_Snapshot(mobileGlobalId, activateAMobileOrderReference);
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            var deserialized = JsonSerializer.Deserialize<T>(stringResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return deserialized;
        }

        public void Dispose()
        {

        }

        public Scenario1_Snapshot Scenario1_Snapshot { get; private set; }
        public Scenario2_Snapshot Scenario2_Snapshot { get; private set; }
        public Scenario3_Snapshot Scenario3_Snapshot { get; private set; }
        public Scenario4_Snapshot Scenario4_Snapshot { get; private set; }
    }
}
