using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class Scenario2_Snapshot
    {
        public MobileDataEntity ActualMobile { get; set; }
        public SimCards.EventHandlers.Data.SimCardOrder ActualSimCardOrder { get; set; }
        public OrderDataEntity ActualMobileOrder { get; set; }
    }
}
