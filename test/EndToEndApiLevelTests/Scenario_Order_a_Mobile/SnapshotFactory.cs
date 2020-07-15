using EndToEndApiLevelTests.DataAcess;
using FluentAssertions;
using System;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class SnapshotFactory : SnapshotFactoryBase
    {
        private readonly Config config;
        private readonly Data data;

        public SnapshotFactory(Config config, Data data)
        {
            this.config = config;
            this.data = data;
            this.data = data;
        }

        public Step_1_Snapshot Take_Step_1_Snapshot(MobileOrderer.Api.Resources.OrderToAdd orderToAdd, Guid mobileGlobalId, Guid orderAMobileOrderReference)
        {
            // Wait for final action... Mobile ProvisionOrder updated to Sent
            var sentMobileOrder = data.MobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Sent", config.FinalActionCheckDelay);
            sentMobileOrder.Should().NotBeNull("Failed to complete Step 1 final action (Mobile ProvisionOrder updated to Sent)");

            return new Step_1_Snapshot
            {
                OrderToAdd = orderToAdd,
                ActualMobile = Snapshot(data.MobilesData.GetMobileByGlobalId(mobileGlobalId)),
                ActualMobileOrder = Snapshot(sentMobileOrder),
                ActualExternalSimCardOrder = data.ExternalSimCardOrdersData.TryGetExternalSimCardOrder(orderAMobileOrderReference),
                ActualSimCardOrder = data.SimCardsData.TryGetSimCardOrder(orderAMobileOrderReference)
            };
        }

        public Step_2_Snapshot Take_Step_2_Snapshot(Guid mobileGlobalId, Guid orderAMobileOrderReference)
        {
            // Wait for final action... Mobile ProvisionOrder updated to Completed
            var completedMobileOrder = data.MobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Completed", config.FinalActionCheckDelay);
            completedMobileOrder.Should().NotBeNull("Failed to complete Step 2 final action (Mobile ProvisionOrder updated to Completed)");
            
            return new Step_2_Snapshot
            {
                ActualMobileOrder = Snapshot(completedMobileOrder),
                ActualMobile = Snapshot(data.MobilesData.GetMobileByGlobalId(mobileGlobalId)),
                ActualSimCardOrder = Snapshot(data.SimCardsData.TryGetSimCardOrder(orderAMobileOrderReference))
            };
        }

        public Step_3_Snapshot Take_Step_3_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        {
            // Wait for final action... Mobile ActivateOrder updated to Sent
            var sentMobileOrder = data.MobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Sent", config.FinalActionCheckDelay);
            sentMobileOrder.Should().NotBeNull("Failed to complete Step 3 final action (Mobile ActivateOrder updated to sent)");

            return new Step_3_Snapshot
            {
                ActualMobileOrder = Snapshot(sentMobileOrder),
                ActualMobile = Snapshot(data.MobilesData.GetMobileByGlobalId(mobileGlobalId)),
                ActualMobileTelecomsNetworkOrder = Snapshot(data.MobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference)),
                ActualExternalMobileTelecomsNetworkOrder = Snapshot(data.ExternalMobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
            };
        }

        public Step_4_Snapshot Take_Step_4_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        {
            // Wait for final action... Mobile ActivateOrder updated to Completed
            var completedMobileOrder = data.MobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Completed", config.FinalActionCheckDelay);
            completedMobileOrder.Should().NotBeNull("Failed to complete Step 4 final action (Mobile ActivateOrder updated to Completed)");

            return new Step_4_Snapshot
            {
                ActualMobileActivateOrderSnapshot = Snapshot(completedMobileOrder),
                ActualMobile = Snapshot(data.MobilesData.GetMobileByGlobalId(mobileGlobalId)),
                ActualMobileTelecomsNetworkOrderSnapshot = Snapshot(data.MobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
            };
        }
    }
}
