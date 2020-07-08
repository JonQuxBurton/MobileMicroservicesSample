using FluentAssertions;
using Xunit;
using System;

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
            // Check Order is Completed in the SIM Cards database
            scenariosFixture.Scenario2ActualSimCardOrderSnapshot.Should().NotBeNull();
            scenariosFixture.Scenario2ActualSimCardOrderSnapshot.Status.Should().Be("Completed");

            // Check Mobiles database
            scenariosFixture.Scenario2MobileSnapshot.Should().NotBeNull();
            scenariosFixture.Scenario2MobileSnapshot.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.WaitingForActivation));

            scenariosFixture.Scenario2ActualMobileOrderSnapshot.Should().NotBeNull();
            scenariosFixture.Scenario2ActualMobileOrderSnapshot.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Order.State), MobileOrderer.Api.Domain.Order.State.Completed));
        }
    }
}
