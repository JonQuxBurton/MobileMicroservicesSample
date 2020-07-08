using FluentAssertions;
using Xunit;
using System;

namespace EndToEndApiLevelTests
{
    [Collection("Scenarios collection")]
    public class Scenario_4_Complete_Activate_Order
    {
        private readonly ScenariosFixture scenariosFixture;

        public Scenario_4_Complete_Activate_Order(ScenariosFixture scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            // Check the Mobile has been updated to Live
            scenariosFixture.Scenario4ActualMobileSnapshot.Should().NotBeNull();
            scenariosFixture.Scenario4ActualMobileSnapshot.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.Live));

            // Check the Activation Order has updated to Completed
            scenariosFixture.Scenario4ActualMobileActivateOrderSnapshot.Should().NotBeNull();
            scenariosFixture.Scenario4ActualMobileActivateOrderSnapshot.State.Should().Be("Completed");

            // Check the MobileTelecomsNetwork Order has updated to Completed
            scenariosFixture.Scenario4MobileTelecomsNetworkOrderSnapshot.Should().NotBeNull();
            scenariosFixture.Scenario4MobileTelecomsNetworkOrderSnapshot.Status.Should().Be("Completed");
        }
    }
}
