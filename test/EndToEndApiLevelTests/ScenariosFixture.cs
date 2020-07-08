using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class ScenariosFixture : IDisposable
    {
        public ScenariosFixture()
        {
            Execute().Wait();
        }

        private async Task Execute()
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";

            var mobilesData = new MobilesData(connectionString);
            var simCardsData = new SimCardsData(connectionString);
            var externalSimCardOrdersData = new ExternalSimCardOrders(connectionString);
            var mobileTelecomsNetworkData = new MobileTelecomsNetworkData(connectionString);
            var externalMobileTelecomsNetworkData = new ExternalMobileTelecomsNetworkData(connectionString);

            var finalActionCheckDelay = TimeSpan.FromSeconds(10);

            // Scneario 1 Order a Mobile
            var client = new HttpClient();
            var url = "http://localhost:5000/api/provisioner";

            Scenario1OrderToAdd = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage actualResponse = await client.PostAsJsonAsync(url, Scenario1OrderToAdd);
            var stringResponse = await actualResponse.Content.ReadAsStringAsync();
            var actualMobileReturned = JsonSerializer.Deserialize<MobileOrderer.Api.Resources.MobileResource>(stringResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobileReturned.Id);

            // Take Scenario 1 Snapshot
            
            // Wait for final action... MobileOrder updated to Sent
            var scenario1_sentMobileOrder = mobilesData.TryGetSentMobileOrder(actualMobileOrder.GlobalId, finalActionCheckDelay);
            scenario1_sentMobileOrder.Should().NotBeNull("Failed to complete Scenario 1 final action (MobileOrder updated to Sent)");

            Scenario1ActualMobileOrderSnapshot = Snapshot(scenario1_sentMobileOrder);
            Scenario1ExternalSimCardOrder = externalSimCardOrdersData.TryGetExternalSimCardOrder(actualMobileOrder.GlobalId);
            Scenario1ActualMobileSnapshot = Snapshot(mobilesData.GetMobile(actualMobileReturned.GlobalId));
            Scenario1ActualSimCardOrder = simCardsData.TryGetSimCardOrder(Scenario1ActualMobileOrderSnapshot.GlobalId);

            var mobileGlobalId = actualMobileReturned.GlobalId;
            var currentMobile = mobilesData.GetMobile(mobileGlobalId);
            var currentMobileOrder = mobilesData.GetMobileOrder(currentMobile.Id);

            // Scenario 2 Complete Mobile Order
            var externalSimCardWholesalerUrl = "http://localhost:5001/api/orders/complete";
            var orderToComplete = new SimCardWholesaler.Api.Resources.OrderToComplete
            {
                Reference = currentMobileOrder.GlobalId
            };

            HttpResponseMessage actualCompleteOrderResponse = await client.PostAsJsonAsync(externalSimCardWholesalerUrl, orderToComplete);

            actualCompleteOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Scenario 2 Snapshot

            // Wait for final action... MobileOrder updated to Completed
            var scenario2_completedMobileOrder = mobilesData.TryGetCompletedMobileOrder(currentMobileOrder.GlobalId, finalActionCheckDelay);
            scenario2_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 2 final action (MobileOrder updated to Completed)");

            Scenario2ActualMobileOrderSnapshot = Snapshot(scenario2_completedMobileOrder);
            Scenario2MobileSnapshot = Snapshot(mobilesData.GetMobile(mobileGlobalId));
            Scenario2ActualSimCardOrderSnapshot = Snapshot(simCardsData.TryGetSimCardOrder(currentMobileOrder.GlobalId));

            // Scenario 3 Activate a Mobile
            var activateTheMobileUrl = $"http://localhost:5000/api/mobiles/{mobileGlobalId}/activate";
            var activateTheMobileOrder = new MobileOrderer.Api.Resources.OrderToAdd
            {
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789"
            };

            HttpResponseMessage actualActivateTheMobileResponse = await client.PostAsJsonAsync(activateTheMobileUrl, activateTheMobileOrder);

            actualActivateTheMobileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var stringResponse2 = await actualActivateTheMobileResponse.Content.ReadAsStringAsync();
            var actualActivationOrderReturned = JsonSerializer.Deserialize<MobileOrderer.Api.Resources.OrderResource>(stringResponse2, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            // Take Scenario 3 Snapshot

            // Wait for final action... Mobile ActivateOrder updated to Sent
            var scenario3_sentMobileOrder = mobilesData.TryGetSentMobileOrder(actualActivationOrderReturned.GlobalId, finalActionCheckDelay);
            scenario3_sentMobileOrder.Should().NotBeNull("Failed to complete Scenario 3 final action (MobileOrder updated to sent)");
            
            Scenario3ActualMobileOrderSnapshot = Snapshot(scenario3_sentMobileOrder);
            Scenario3MobileSnapshot = Snapshot(mobilesData.GetMobile(mobileGlobalId));
            Scenario3MobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(actualActivationOrderReturned.GlobalId));
            Scenario3ExternalMobileTelecomsNetworkOrder = Snapshot(externalMobileTelecomsNetworkData.TryGetOrder(actualActivationOrderReturned.GlobalId));

            // Scenario 4 Complete Activate Order
            var externalMobileTelecomsNetworkUrl = "http://localhost:5002/api/orders/complete";
            var activateOrderToComplete = new ExternalMobileTelecomsNetwork.Api.Resources.OrderToComplete
            {
                Reference = actualActivationOrderReturned.GlobalId
            };

            HttpResponseMessage actualCompleteActivateOrderResponse = await client.PostAsJsonAsync(externalMobileTelecomsNetworkUrl, activateOrderToComplete);

            actualCompleteActivateOrderResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Take Scenario 4 Snapshot

            // Wait for final action... Mobile ActivateOrder updated to Completed
            var scenario4_completedMobileOrder = mobilesData.TryGetCompletedMobileOrder(actualActivationOrderReturned.GlobalId, finalActionCheckDelay);
            scenario4_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 4 final action (MobileOrder updated to Completed)");

            Scenario4ActualMobileActivateOrderSnapshot = Snapshot(scenario4_completedMobileOrder);
            Scenario4ActualMobileSnapshot = Snapshot(mobilesData.GetMobile(mobileGlobalId));
            Scenario4MobileTelecomsNetworkOrderSnapshot = Snapshot(mobileTelecomsNetworkData.TryGetOrder(actualActivationOrderReturned.GlobalId));
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

        public MobileOrderer.Api.Resources.OrderToAdd Scenario1OrderToAdd { get; private set; }
        public MobileDataEntity Scenario1ActualMobileSnapshot { get; private set; }
        public SimCards.EventHandlers.Data.SimCardOrder Scenario1ActualSimCardOrder { get; private set; }
        public OrderDataEntity Scenario1ActualMobileOrderSnapshot { get; private set; }
        public SimCardWholesaler.Api.Data.Order Scenario1ExternalSimCardOrder { get; private set; }

        public MobileDataEntity Scenario2MobileSnapshot { get; private set; }
        public SimCards.EventHandlers.Data.SimCardOrder Scenario2ActualSimCardOrderSnapshot { get; private set; }
        public OrderDataEntity Scenario2ActualMobileOrderSnapshot { get; private set; }

        public OrderDataEntity Scenario3ActualMobileOrderSnapshot { get; private set; }
        public MobileDataEntity Scenario3MobileSnapshot { get; private set; }
        public MobileTelecomsNetwork.EventHandlers.Data.ActivationOrder Scenario3MobileTelecomsNetworkOrder { get; private set; }
        public ExternalMobileTelecomsNetwork.Api.Data.Order Scenario3ExternalMobileTelecomsNetworkOrder { get; private set; }


        public MobileDataEntity Scenario4ActualMobileSnapshot { get; private set; }
        public OrderDataEntity Scenario4ActualMobileActivateOrderSnapshot { get; private set; }
        public MobileTelecomsNetwork.EventHandlers.Data.ActivationOrder Scenario4MobileTelecomsNetworkOrderSnapshot { get; private set; }
    }
}
