using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class Scenario3_Snapshot
    {
        public OrderDataEntity ActualMobileOrder { get; set; }
        public MobileDataEntity ActualMobile { get; set; }
        public MobileTelecomsNetwork.EventHandlers.Data.Order ActualMobileTelecomsNetworkOrder { get; set; }
        public ExternalMobileTelecomsNetwork.Api.Data.Order ActualExternalMobileTelecomsNetworkOrder { get; set; }
    }
}
