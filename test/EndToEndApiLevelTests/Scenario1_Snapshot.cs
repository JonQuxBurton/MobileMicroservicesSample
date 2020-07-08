using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests
{
    public class Scenario1_Snapshot
    {
        public MobileOrderer.Api.Resources.OrderToAdd OrderToAdd { get; set; }
        public MobileDataEntity ActualMobile { get; set; }
        public SimCards.EventHandlers.Data.SimCardOrder ActualSimCardOrder { get; set; }
        public OrderDataEntity ActualMobileOrder { get; set; }
        public SimCardWholesaler.Api.Data.Order ActualExternalSimCardOrder { get; set; }
    }
}
