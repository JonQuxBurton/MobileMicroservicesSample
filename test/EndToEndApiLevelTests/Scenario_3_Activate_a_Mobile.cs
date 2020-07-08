using FluentAssertions;
using Xunit;
using System;

namespace EndToEndApiLevelTests
{
    [Collection("Scenarios collection")]
    public class Scenario_3_Activate_a_Mobile
    {
        private readonly ScenariosFixture scenariosFixture;

        public Scenario_3_Activate_a_Mobile(ScenariosFixture scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            // Check the Mobile has been updated to Processing Activation
            scenariosFixture.Scenario3MobileSnapshot.Should().NotBeNull();
            //scenariosFixture.Scenario3MobileState.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.ProcessingActivation));
            scenariosFixture.Scenario3MobileSnapshot.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.ProcessingActivation));

            // Check the Mobile ActivateOrder has been updated to Sent
            scenariosFixture.Scenario3ActualMobileOrderSnapshot.State.Should().Be("Sent");

            // Check the Activation Order has been sent to the External Mobile Telecoms Network
            scenariosFixture.Scenario3MobileTelecomsNetworkOrder.Should().NotBeNull();
            scenariosFixture.Scenario3MobileTelecomsNetworkOrder.Status.Should().Be("Sent");

            // Check the Activation Order has been recevied by the External Mobile Telecoms Network
            scenariosFixture.Scenario3ExternalMobileTelecomsNetworkOrder.Should().NotBeNull();
            scenariosFixture.Scenario3ExternalMobileTelecomsNetworkOrder.Status.Should().Be("New");
        }
    }
}
