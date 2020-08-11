using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Cease_a_Mobile
{
    public class Step_2_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public OrderDataEntity ActualMobileOrder { get; set; }

        public MobileTelecomsNetwork.EventHandlers.Domain.Order ActualMobileTelecomsNetworkOrder { get; set; }
    }
}