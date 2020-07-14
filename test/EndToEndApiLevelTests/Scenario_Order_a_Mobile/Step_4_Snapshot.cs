using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class Step_4_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public OrderDataEntity ActualMobileActivateOrderSnapshot { get; set; }
        public MobileTelecomsNetwork.EventHandlers.Data.Order ActualMobileTelecomsNetworkOrderSnapshot { get; set; }
    }
}
