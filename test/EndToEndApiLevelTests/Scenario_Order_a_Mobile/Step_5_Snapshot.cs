using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class Step_5_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public OrderDataEntity ActualMobileActivateOrderSnapshot { get; set; }
        public MobileTelecomsNetwork.EventHandlers.Domain.Order ActualMobileTelecomsNetworkOrderSnapshot { get; set; }
    }
}
