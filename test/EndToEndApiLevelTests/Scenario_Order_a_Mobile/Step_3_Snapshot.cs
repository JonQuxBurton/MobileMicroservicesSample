using Mobiles.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class Step_3_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public SimCards.EventHandlers.Data.SimCardOrder ActualSimCardOrder { get; set; }
        public OrderDataEntity ActualMobileOrder { get; set; }
    }
}
