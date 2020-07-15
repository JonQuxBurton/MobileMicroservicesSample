using FluentAssertions;
using MobileOrderer.Api.Domain;
using EndToEndApiLevelTests.DataAcess;

namespace EndToEndApiLevelTests.Scenario_Cease_a_Mobile
{
    public class SnapshotFactory : SnapshotFactoryBase
    {
        private readonly Config config;
        private readonly Data data;

        public SnapshotFactory(Config config, Data data)
        {
            this.config = config;
            this.data = data;
        }

        public Step_1_Snapshot Take_Step_1_Snapshot(MobileDataEntity mobile)
        {
            var currentMobile = data.MobilesData.GetMobileByGlobalId(mobile.GlobalId);
            var currentMobileOrder = data.MobilesData.GetMobileOrder(currentMobile.Id);

            // Wait for final action... Mobile CeaseOrder updated to Sent
            var finalMobileOrder = data.MobilesData.TryGetMobileOrderInState(currentMobileOrder.GlobalId, "Sent", config.FinalActionCheckDelay);
            finalMobileOrder.Should().NotBeNull("Failed to complete Step 1 final action (Mobile CeaseOrder updated to Sent)");

            return new Step_1_Snapshot
            {
                ActualMobile = Snapshot(data.MobilesData.GetMobileByGlobalId(mobile.GlobalId)),
                ActualMobileOrder = Snapshot(finalMobileOrder),
                ActualMobileTelecomsNetworkOrder = Snapshot(data.MobileTelecomsNetworkData.TryGetOrder(currentMobileOrder.GlobalId)),
                ActualExternalMobileTelecomsNetworkOrder = Snapshot(data.ExternalMobileTelecomsNetworkData.TryGetOrder(currentMobileOrder.GlobalId)),
            };
        }
        
        public Step_2_Snapshot Take_Step_2_Snapshot(MobileDataEntity mobile)
        {
            var currentMobile = data.MobilesData.GetMobileByGlobalId(mobile.GlobalId);
            currentMobile.Orders = null;
            var currentMobileOrder = data.MobilesData.GetMobileOrder(currentMobile.Id);

            // Wait for final action... Mobile CeaseOrder updated to Completed
            var finalMobileOrder = data.MobilesData.TryGetMobileOrderInState(currentMobileOrder.GlobalId, "Completed", config.FinalActionCheckDelay);
            finalMobileOrder.Should().NotBeNull("Failed to complete Step 2 final action (Mobile CeaseOrder updated to 'Completed')");

            return new Step_2_Snapshot
            {
                ActualMobile = Snapshot(data.MobilesData.GetMobileByGlobalId(mobile.GlobalId)),
                ActualMobileOrder = Snapshot(finalMobileOrder),
                ActualMobileTelecomsNetworkOrder = Snapshot(data.MobileTelecomsNetworkData.TryGetOrder(currentMobileOrder.GlobalId))
            };
        }
    }
}
