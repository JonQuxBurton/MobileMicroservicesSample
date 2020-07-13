using FluentAssertions;
using System.Text.Json;
using System;
using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Cancel_A_Mobile
{
    public class SnapshotFactory
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

        //public Step_1_Snapshot Take_Step_1_Snapshot(Guid mobileGlobalId, Guid orderAMobileOrderReference)
        public Step_1_Snapshot Take_Step_1_Snapshot(MobileDataEntity mobile)
        {
            //OrderDataEntity orderAMobileOrderReference
            var actualMobile = mobilesData.GetMobile(mobile.GlobalId);
            //var actualMobileOrder = mobilesData.GetMobile(mobile.GlobalId);
            var actualMobileOrder = mobilesData.GetMobileOrder(actualMobile.Id);

            // Wait for final action... MobileCancelOrder updated to Sent
            var scenario1_sentMobileOrder = mobilesData.TryGetMobileOrder(actualMobileOrder.GlobalId, "Sent", finalActionCheckDelay);
            scenario1_sentMobileOrder.Should().NotBeNull("Failed to complete Step 1 final action (MobileCancelOrder updated to Sent)");

            return new Step_1_Snapshot
            {
                ActualMobile = Snapshot(mobile),
                ActualMobileOrder = Snapshot(scenario1_sentMobileOrder),
                //ActualExternalSimCardOrder = externalSimCardOrdersData.TryGetExternalSimCardOrder(orderAMobileOrderReference),
                //ActualSimCardOrder = simCardsData.TryGetSimCardOrder(orderAMobileOrderReference)
            };
        }

        //public Scenario2_Snapshot Take_Scenario2_Snapshot(Guid mobileGlobalId, Guid orderAMobileOrderReference)
        //{
        //    // Wait for final action... MobileOrder updated to Completed
        //    var scenario2_completedMobileOrder = mobilesData.TryGetMobileOrder(orderAMobileOrderReference, "Completed", finalActionCheckDelay);
        //    scenario2_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 2 final action (MobileOrder updated to Completed)");
        //    return new Scenario2_Snapshot
        //    {
        //        ActualMobileOrder = Snapshot(scenario2_completedMobileOrder),
        //        ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
        //        ActualSimCardOrder = Snapshot(simCardsData.TryGetSimCardOrder(orderAMobileOrderReference))
        //    };
        //}

        //public Scenario3_Snapshot Take_Scenario3_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        //{
        //    // Wait for final action... Mobile ActivateOrder updated to Sent
        //    var scenario3_sentMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Sent", finalActionCheckDelay);
        //    scenario3_sentMobileOrder.Should().NotBeNull("Failed to complete Scenario 3 final action (MobileOrder updated to sent)");

        //    return new Scenario3_Snapshot
        //    {
        //        ActualMobileOrder = Snapshot(scenario3_sentMobileOrder),
        //        ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
        //        ActualMobileTelecomsNetworkOrder = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference)),
        //        ActualExternalMobileTelecomsNetworkOrder = Snapshot(externalMobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
        //    };
        //}

        //public Scenario4_Snapshot Take_Scenario4_Snapshot(Guid mobileGlobalId, Guid activateAMobileOrderReference)
        //{
        //    // Wait for final action... Mobile ActivateOrder updated to Completed
        //    var scenario4_completedMobileOrder = mobilesData.TryGetMobileOrder(activateAMobileOrderReference, "Completed", finalActionCheckDelay);
        //    scenario4_completedMobileOrder.Should().NotBeNull("Failed to complete Scenario 4 final action (MobileOrder updated to Completed)");

        //    return new Scenario4_Snapshot
        //    {
        //        ActualMobileActivateOrderSnapshot = Snapshot(scenario4_completedMobileOrder),
        //        ActualMobile = Snapshot(mobilesData.GetMobile(mobileGlobalId)),
        //        ActualMobileTelecomsNetworkOrderSnapshot = Snapshot(mobileTelecomsNetworkData.TryGetOrder(activateAMobileOrderReference))
        //    };
        //}

        private T Snapshot<T>(T original) where T : class
        {
            if (original == null)
                return null;

            var serialized = JsonSerializer.Serialize(original);
            return JsonSerializer.Deserialize<T>(serialized);
        }
    }
}
