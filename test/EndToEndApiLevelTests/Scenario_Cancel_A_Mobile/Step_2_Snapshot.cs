using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Cancel_A_Mobile
{
    public class Step_2_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public OrderDataEntity ActualMobileOrder { get; set; }

        public MobileTelecomsNetwork.EventHandlers.Data.Order ActualMobileTelecomsNetworkOrder { get; set; }
    }
}