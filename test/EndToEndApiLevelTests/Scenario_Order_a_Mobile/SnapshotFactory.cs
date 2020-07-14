using EndToEndApiLevelTests.Data;
using FluentAssertions;
using System;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class SnapshotFactory : SnapshotFactoryBase
    {
        private readonly TimeSpan finalActionCheckDelay;
        private readonly MobilesData mobilesData;
        private readonly SimCardsData simCardsData;
        private readonly ExternalSimCardOrdersData externalSimCardOrdersData;
        private readonly MobileTelecomsNetworkData mobileTelecomsNetworkData;
        private readonly ExternalMobileTelecomsNetworkData externalMobileTelecomsNetworkData;

        public SnapshotFactory(string connectionString, TimeSpan finalActionCheckDelay)
        {
            this.finalActionCheckDelay = finalActionCheckDelay;

            mobilesData = new MobilesData(connectionString);
            simCardsData = new SimCardsData(connectionString);
            externalSimCardOrdersData = new ExternalSimCardOrdersData(connectionString);
            mobileTelecomsNetworkData = new MobileTelecomsNetworkData(connectionString);
            externalMobileTelecomsNetworkData = new ExternalMobileTelecomsNetworkData(connectionString);
        }

        public Step_1_Snapshot Take_Step_1_Snapshot(MobileOrderer.Api.Resources.OrderToAdd orderToAdd, Guid mobileGlobalId, Guid orderAMobileOrderReference)
        {
            // Wait for final action... Mobile ProvisionOrder updated to Sent
            var sentMobileOrder = mobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Sent", finalActionCheckDelay);
            sentMobileOrder.Should().NotBeNull("Failed to complete Step 1 final action (Mobile ProvisionOrder updated to Sent)");

            return new Step_1_Snapshot
            {
                OrderToAdd = orderToAdd,
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualMobileOrder = Snapshot(sentMobileOrder),
                ActualExternalSimCardOrder = externalSimCardOrdersData.TryGetExternalSimCardOrder(orderAMobileOrderReference),
                ActualSimCardOrder = simCardsData.TryGetSimCardOrder(orderAMobileOrderReference)
            };
        }

        public Step_2_Snapshot Take_Step_2_Snapshot(Guid mobileGlobalId, Guid orderAMobileOrderReference)
        {
            // Wait for final action... Mobile ProvisionOrder updated to Completed
            var completedMobileOrder = mobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Completed", finalActionCheckDelay);
            completedMobileOrder.Should().NotBeNull("Failed to complete Step 2 final action (Mobile ProvisionOrder updated to Completed)");
            
            return new Step_2_Snapshot
            {
                ActualMobileOrder = Snapshot(completedMobileOrder),
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualSimCardOrder = Snapshot(simCardsData.TryGetSimCardOrder(orderAMobileOrderReference))
            };
        }

        public Step_3_Snapshot Take_Step_3_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        {
            // Wait for final action... Mobile ActivateOrder updated to Sent
            var sentMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Sent", finalActionCheckDelay);
            sentMobileOrder.Should().NotBeNull("Failed to complete Step 3 final action (Mobile ActivateOrder updated to sent)");

            return new Step_3_Snapshot
            {
                ActualMobileOrder = Snapshot(sentMobileOrder),
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualMobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference)),
                ActualExternalMobileTelecomsNetworkOrder = Snapshot(externalMobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
            };
        }

        public Step_4_Snapshot Take_Step_4_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        {
            // Wait for final action... Mobile ActivateOrder updated to Completed
            var completedMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Completed", finalActionCheckDelay);
            completedMobileOrder.Should().NotBeNull("Failed to complete Step 4 final action (Mobile ActivateOrder updated to Completed)");

            return new Step_4_Snapshot
            {
                ActualMobileActivateOrderSnapshot = Snapshot(completedMobileOrder),
                ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
                ActualMobileTelecomsNetworkOrderSnapshot = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
            };
        }
    }
}
