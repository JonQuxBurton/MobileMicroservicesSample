using FluentAssertions;
using Xunit;
using System;
using Utils.Enums;

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
            var snapshot = scenariosFixture.Scenario3_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.ProcessingActivation);

            // Check the Mobile has been updated to Processing Activation
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check the Mobile ActivateOrder has been updated to Sent
            snapshot.ActualMobileOrder.State.Should().Be("Sent");

            // Check the Activation Order has been sent to the External Mobile Telecoms Network
            snapshot.ActualMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be("Sent");

            // Check the Activation Order has been recevied by the External Mobile Telecoms Network
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Status.Should().Be("New");
        }
    }
}
