using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests
{
    [Collection("Scenarios collection")]
    public class Scenario_2_Complete_Mobile_Order
    {
        private readonly ScenariosFixture scenariosFixture;

        public Scenario_2_Complete_Mobile_Order(ScenariosFixture scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = scenariosFixture.Scenario2_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.WaitingForActivation);
            var expectedMobileOrderState = enumConverter.ToName<MobileOrderer.Api.Domain.Order.State>(MobileOrderer.Api.Domain.Order.State.Completed);

            // Check Order is Completed in the SIM Cards database
            snapshot.ActualSimCardOrder.Should().NotBeNull();
            snapshot.ActualSimCardOrder.Status.Should().Be("Completed");

            // Check Mobiles database
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.State.Should().Be(expectedMobileOrderState);
        }
    }
}
