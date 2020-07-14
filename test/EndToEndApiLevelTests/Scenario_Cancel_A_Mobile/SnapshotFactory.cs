﻿using FluentAssertions;
using System;
using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Cancel_A_Mobile
{
    public class SnapshotFactory : SnapshotFactoryBase
    {
        private readonly TimeSpan finalActionCheckDelay;
        private readonly MobilesData mobilesData;
        private readonly SimCardsData simCardsData;
        private readonly ExternalSimCardOrders externalSimCardOrdersData;
        private readonly MobileTelecomsNetworkData mobileTelecomsNetworkData;
        private readonly ExternalMobileTelecomsNetworkData externalMobileTelecomsNetworkData;

        public SnapshotFactory(string connectionString, TimeSpan finalActionCheckDelay)
        {
            this.finalActionCheckDelay = finalActionCheckDelay;

            mobilesData = new MobilesData(connectionString);
            simCardsData = new SimCardsData(connectionString);
            externalSimCardOrdersData = new ExternalSimCardOrders(connectionString);
            mobileTelecomsNetworkData = new MobileTelecomsNetworkData(connectionString);
            externalMobileTelecomsNetworkData = new ExternalMobileTelecomsNetworkData(connectionString);
        }

        public Step_1_Snapshot Take_Step_1_Snapshot(MobileDataEntity mobile)
        {
            var currentMobile = mobilesData.GetMobile(mobile.GlobalId);
            var currentMobileOrder = mobilesData.GetMobileOrder(currentMobile.Id);

            // Wait for final action... MobileCancelOrder updated to Sent
            var finalMobileOrder = mobilesData.TryGetMobileOrder(currentMobileOrder.GlobalId, "Sent", finalActionCheckDelay);
            finalMobileOrder.Should().NotBeNull("Failed to complete Step 1 final action (MobileCancelOrder updated to Sent)");

            return new Step_1_Snapshot
            {
                ActualMobile = Snapshot(mobilesData.GetMobile(mobile.GlobalId)),
                ActualMobileOrder = Snapshot(finalMobileOrder),
                ActualMobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(currentMobileOrder.GlobalId)),
                ActualExternalMobileTelecomsNetworkOrder = Snapshot(externalMobileTelecomsNetworkData.TryGetOrder(currentMobileOrder.GlobalId)),
            };
        }
        
        public Step_2_Snapshot Take_Step_2_Snapshot(MobileDataEntity mobile)
        {
            var currentMobile = mobilesData.GetMobile(mobile.GlobalId);
            var currentMobileOrder = mobilesData.GetMobileOrder(currentMobile.Id);

            // Wait for final action... Mobile CancelOrder updated to Completed
            var finalMobileOrder = mobilesData.TryGetMobileOrder(currentMobileOrder.GlobalId, "Completed", finalActionCheckDelay);
            finalMobileOrder.Should().NotBeNull("Failed to complete Step 2 final action (Mobile CancelOrder updated to 'Completed')");

            return new Step_2_Snapshot
            {
                ActualMobile = Snapshot(mobilesData.GetMobile(mobile.GlobalId)),
                ActualMobileOrder = Snapshot(finalMobileOrder),
                ActualMobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(currentMobileOrder.GlobalId))
            };
        }
    }
}
