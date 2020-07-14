using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    [Collection("Scenario Order_a_Mobile collection")]
    public class Step_3_Activate_a_Mobile
    {
        private readonly Scenario_Order_a_Mobile_Fixture fixture;

        public Step_3_Activate_a_Mobile(Scenario_Order_a_Mobile_Fixture fixture)
        {
            this.fixture = fixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = fixture.Step_3_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.ProcessingActivation);

            // Check the Mobile has been updated to ProcessingActivation
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
