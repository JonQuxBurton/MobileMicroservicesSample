using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using EndToEndApiLevelTests.Scenario_Order_a_Mobile;
using EndToEndApiLevelTests.Data;

namespace EndToEndApiLevelTests
{
    public partial class Scenario_Order_a_Mobile_Fixture : IDisposable
    {
        public Step_1_Snapshot Step_1_Snapshot { get; private set; }
        public Step_2_Snapshot Step_2_Snapshot { get; private set; }
        public Step_3_Snapshot Step_3_Snapshot { get; private set; }
        public Step_4_Snapshot Step_4_Snapshot { get; private set; }

        public Scenario_Order_a_Mobile_Fixture()
        {
            Execute().Wait();
        }

        private async Task Execute()
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            var mobilesData = new MobilesData(connectionString);

            var snapshotFactory = new SnapshotFactory(connectionString, TimeSpan.FromSeconds(10));

            // Step 1 Order a Mobile
            var client = new HttpClient();
            var url = "http://localhost:5000/api/provisioner";

            var orderToAdd = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage orderAMobileResponse = await client.PostAsJsonAsync(url, orderToAdd);

            orderAMobileResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Gather needed data
            var actualMobileReturned = await DeserializeResponse<MobileOrderer.Api.Resources.MobileResource>(orderAMobileResponse);
            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobileReturned.Id);
            var mobileGlobalId = actualMobileReturned.GlobalId;
            var orderAMobileOrderReference = actualMobileOrder.GlobalId;

            // Take Step 1 Snapshot
            Step_1_Snapshot = snapshotFactory.Take_Step_1_Snapshot(orderToAdd, mobileGlobalId, orderAMobileOrderReference);

            // Step 2 Complete Mobile Order
            var externalSimCardWholesalerUrl = $"http://localhost:5001/api/orders/{orderAMobileOrderReference}/complete";

            HttpResponseMessage actualCompleteOrderResponse = await client.PostAsync(externalSimCardWholesalerUrl, null);

            actualCompleteOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Step 2 Snapshot
            Step_2_Snapshot = snapshotFactory.Take_Step_2_Snapshot(mobileGlobalId, orderAMobileOrderReference);

            // Step 3 Activate a Mobile
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

            // Take Step 3 Snapshot
            Step_3_Snapshot = snapshotFactory.Take_Step_3_Snapshot(mobileGlobalId, activateAMobileOrderReference);

            // Step 4 Complete Activate Order
            var externalMobileTelecomsNetworkUrl = $"http://localhost:5002/api/orders/{activateAMobileOrderReference}/complete";

            HttpResponseMessage actualCompleteActivateOrderResponse = await client.PostAsync(externalMobileTelecomsNetworkUrl, null);

            actualCompleteActivateOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Step 4 Snapshot
            Step_4_Snapshot = snapshotFactory.Take_Step_4_Snapshot(mobileGlobalId, activateAMobileOrderReference);
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

    }
}
