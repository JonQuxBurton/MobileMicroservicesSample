using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class Scenario4_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public OrderDataEntity ActualMobileActivateOrderSnapshot { get; set; }
        public MobileTelecomsNetwork.EventHandlers.Data.Order ActualMobileTelecomsNetworkOrderSnapshot { get; set; }
    }
}
