using MobileOrderer.Api.Domain;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    public class Step_1_Snapshot
    {
        public MobileOrderer.Api.Resources.CustomerToAdd CustomerToAdd { get; set; }
        public Customer ActualCustomer { get; set; }
    }
}
