using Mobiles.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class Step_4_Snapshot
    {
        public OrderDataEntity ActualMobileOrder { get; set; }
        public MobileDataEntity ActualMobile { get; set; }
        public MobileTelecomsNetwork.EventHandlers.Domain.Order ActualMobileTelecomsNetworkOrder { get; set; }
        public ExternalMobileTelecomsNetwork.Api.Data.Order ActualExternalMobileTelecomsNetworkOrder { get; set; }
    }
}
