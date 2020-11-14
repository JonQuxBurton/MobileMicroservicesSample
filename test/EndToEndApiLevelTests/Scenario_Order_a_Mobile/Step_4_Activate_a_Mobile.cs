using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    [Collection("Scenario Order_a_Mobile collection")]
    public class Step_4_Activate_a_Mobile
    {
        private readonly Scenario_Order_a_Mobile_Script fixture;

        public Step_4_Activate_a_Mobile(Scenario_Order_a_Mobile_Script fixture)
        {
            this.fixture = fixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = fixture.Step_4_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<Mobiles.Api.Domain.Mobile.MobileState>(Mobiles.Api.Domain.Mobile.MobileState.ProcessingActivate);

            // Check the Mobile has been updated to ProcessingActivate
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check the Mobile ActivateOrder has been updated to Sent
            snapshot.ActualMobileOrder.State.Should().Be("Sent");

            // Check the Activate Order has been sent to the External Mobile Telecoms Network
            snapshot.ActualMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be(MobileTelecomsNetwork.EventHandlers.Domain.OrderStatus.Sent);

            // Check the Activate Order has been received by the External Mobile Telecoms Network
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Status.Should().Be("New");
        }
    }
}
