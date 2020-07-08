using FluentAssertions;
using System.Text.Json;
using System;

namespace EndToEndApiLevelTests
{
    public class Snapshot_Factory
    {


        public static Scenario1_Snapshot Take_Scenario1_Snapshot(MobileOrderer.Api.Resources.OrderToAdd scenario1OrderToAdd, Guid mobileGlobalId, Guid orderAMobileOrderReference)
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";

            var mobilesData = new MobilesData(connectionString);
            var simCardsData = new SimCardsData(connectionString);
            var externalSimCardOrdersData = new ExternalSimCardOrders(connectionString);

            var finalActionCheckDelay = TimeSpan.FromSeconds(10);

            // Wait for final action... MobileOrder updated to Sent
            var scenario1_sentMobileOrder = mobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Sent", finalActionCheckDelay);
            scenario1_sentMobileOrder.Should().NotBeNull("Failed to complete Scenario 1 final action (MobileOrder updated to Sent)");

            return new Scenario1_Snapshot
            {
                OrderToAdd = scenario1OrderToAdd,
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualMobileOrder = Snapshot(scenario1_sentMobileOrder),
                ActualExternalSimCardOrder = externalSimCardOrdersData.TryGetExternalSimCardOrder(orderAMobileOrderReference),
                ActualSimCardOrder = simCardsData.TryGetSimCardOrder(orderAMobileOrderReference)
            };
        }

        public static Scenario2_Snapshot Take_Scenario2_Snapshot(Guid mobileGlobalId, Guid orderAMobileOrderReference)
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";

            var mobilesData = new MobilesData(connectionString);
            var simCardsData = new SimCardsData(connectionString);
            
            var finalActionCheckDelay = TimeSpan.FromSeconds(10);

            // Wait for final action... MobileOrder updated to Completed
            var scenario2_completedMobileOrder = mobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Completed", finalActionCheckDelay);
            scenario2_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 2 final action (MobileOrder updated to Completed)");
            return new Scenario2_Snapshot
            {
                ActualMobileOrder = Snapshot(scenario2_completedMobileOrder),
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualSimCardOrder = Snapshot(simCardsData.TryGetSimCardOrder(orderAMobileOrderReference))
            };
        }

        public static Scenario3_Snapshot Take_Scenario3_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            var mobileTelecomsNetworkData = new MobileTelecomsNetworkData(connectionString);
            var externalMobileTelecomsNetworkData = new ExternalMobileTelecomsNetworkData(connectionString);

            var finalActionCheckDelay = TimeSpan.FromSeconds(10);

            var mobilesData = new MobilesData(connectionString);

            // Wait for final action... Mobile ActivateOrder updated to Sent
            var scenario3_sentMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Sent", finalActionCheckDelay);
            scenario3_sentMobileOrder.Should().NotBeNull("Failed to complete Scenario 3 final action (MobileOrder updated to sent)");

            return new Scenario3_Snapshot
            {
                ActualMobileOrder = Snapshot(scenario3_sentMobileOrder),
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualMobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference)),
                ActualExternalMobileTelecomsNetworkOrder = Snapshot(externalMobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
            };
        }

        public static Scenario4_Snapshot Take_Scenario4_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        {
            var connectionString = "Server=localhost,5433;Database=Mobile;User Id=SA;Password=Pass@word";
            var mobileTelecomsNetworkData = new MobileTelecomsNetworkData(connectionString);

            var finalActionCheckDelay = TimeSpan.FromSeconds(10);

            var mobilesData = new MobilesData(connectionString);

            // Wait for final action... Mobile ActivateOrder updated to Completed
            var scenario4_completedMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Completed", finalActionCheckDelay);
            scenario4_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 4 final action (MobileOrder updated to Completed)");

            return new Scenario4_Snapshot
            {
                ActualMobileActivateOrderSnapshot = Snapshot(scenario4_completedMobileOrder),
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualMobileTelecomsNetworkOrderSnapshot = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
            };
        }

        private static T Snapshot<T>(T original) where T : class
        {
            if (original == null)
                return null;

            var serialized = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<T>(serialized);
        }
    }
}
