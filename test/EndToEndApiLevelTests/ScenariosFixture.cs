using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using MobileOrderer.Api.Domain;

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
            var mobileTelecomsNetworkData = new MobileTelecomsNetworkData(connectionString);
            var externalMobileTelecomsNetworkData = new ExternalMobileTelecomsNetworkData(connectionString);

            var finalActionCheckDelay = TimeSpan.FromSeconds(10);

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
            scenario1_Snapshot = Snapshot_Factory.Take_Scenario1_Snapshot(scenario1OrderToAdd, mobileGlobalId, orderAMobileOrderReference);

            // Scenario 2 Complete Mobile Order
            var externalSimCardWholesalerUrl = "http://localhost:5001/api/orders/complete";
            var orderToComplete = new SimCardWholesaler.Api.Resources.OrderToComplete
            {
                Reference = orderAMobileOrderReference
            };

            HttpResponseMessage actualCompleteOrderResponse = await client.PostAsJsonAsync(externalSimCardWholesalerUrl, orderToComplete);

            actualCompleteOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Scenario 2 Snapshot
            scenario2_Snapshot = Snapshot_Factory.Take_Scenario2_Snapshot(mobileGlobalId, orderAMobileOrderReference);

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

            // Wait for final action... Mobile ActivateOrder updated to Sent
            var scenario3_sentMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Sent", finalActionCheckDelay);
            scenario3_sentMobileOrder.Should().NotBeNull("Failed to complete Scenario 3 final action (MobileOrder updated to sent)");
            
            Scenario3ActualMobileOrderSnapshot = Snapshot(scenario3_sentMobileOrder);
            Scenario3MobileSnapshot = Snapshot(mobilesData.GetMobile(mobileGlobalId));
            Scenario3MobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference));
            Scenario3ExternalMobileTelecomsNetworkOrder = Snapshot(externalMobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference));

            // Scenario 4 Complete Activate Order
            var externalMobileTelecomsNetworkUrl = "http://localhost:5002/api/orders/complete";
            var activateOrderToComplete = new ExternalMobileTelecomsNetwork.Api.Resources.OrderToComplete
            {
                Reference = activateAMobileOrderReference
            };

            HttpResponseMessage actualCompleteActivateOrderResponse = await client.PostAsJsonAsync(externalMobileTelecomsNetworkUrl, activateOrderToComplete);

            actualCompleteActivateOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Scenario 4 Snapshot

            // Wait for final action... Mobile ActivateOrder updated to Completed
            var scenario4_completedMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Completed", finalActionCheckDelay);
            scenario4_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 4 final action (MobileOrder updated to Completed)");

            Scenario4ActualMobileActivateOrderSnapshot = Snapshot(scenario4_completedMobileOrder);
            Scenario4ActualMobileSnapshot = Snapshot(mobilesData.GetMobile(mobileGlobalId));
            Scenario4MobileTelecomsNetworkOrderSnapshot = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference));
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var stringResponse = await response.Content.ReadAsStringAsync();
            var deserialized = JsonSerializer.Deserialize<T>(stringResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return deserialized;
        }

        private T Snapshot<T>(T original) where T: class
        {
            if (original == null)
                return null;

            var serialized = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<T>(serialized);
        }

        public void Dispose()
        {

        }

        public Scenario1_Snapshot scenario1_Snapshot { get; private set; }
        public Scenario2_Snapshot scenario2_Snapshot { get; private set; }


        public OrderDataEntity Scenario3ActualMobileOrderSnapshot { get; private set; }
        public MobileDataEntity Scenario3MobileSnapshot { get; private set; }
        public MobileTelecomsNetwork.EventHandlers.Data.ActivationOrder Scenario3MobileTelecomsNetworkOrder { get; private set; }
        public ExternalMobileTelecomsNetwork.Api.Data.Order Scenario3ExternalMobileTelecomsNetworkOrder { get; private set; }


        public MobileDataEntity Scenario4ActualMobileSnapshot { get; private set; }
        public OrderDataEntity Scenario4ActualMobileActivateOrderSnapshot { get; private set; }
        public MobileTelecomsNetwork.EventHandlers.Data.ActivationOrder Scenario4MobileTelecomsNetworkOrderSnapshot { get; private set; }
    }
}
