using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    [Collection("Scenario Order_a_Mobile collection")]
    public class Step_3_Provision_Order_has_been_Completed
    {
        private readonly Scenario_Order_a_Mobile_Script fixture;

        public Step_3_Provision_Order_has_been_Completed(Scenario_Order_a_Mobile_Script fixture)
        {
            this.fixture = fixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = fixture.Step_3_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.WaitingForActivation);
            var expectedMobileOrderState = enumConverter.ToName<MobileOrderer.Api.Domain.Order.State>(MobileOrderer.Api.Domain.Order.State.Completed);

            // Check Order is Completed in the SIM Cards database
            snapshot.ActualSimCardOrder.Should().NotBeNull();
            snapshot.ActualSimCardOrder.Status.Should().Be("Completed");

            // Check Mobile has been updated
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check Mobile Order has been updated
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.State.Should().Be(expectedMobileOrderState);
        }
    }
}
