using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class Step_2_Snapshot
    {
        public MobileOrderer.Api.Resources.OrderToAdd OrderToAdd { get; set; }
        public MobileDataEntity ActualMobile { get; set; }
        public SimCards.EventHandlers.Data.SimCardOrder ActualSimCardOrder { get; set; }
        public OrderDataEntity ActualMobileOrder { get; set; }
        public ExternalSimCardsProvider.Api.Data.Order ActualExternalSimCardOrder { get; set; }
        public CustomerResource ActualCustomer { get; set; }
    }
}
