using FluentAssertions;
using Xunit;
using Utils.Enums;
using MobileTelecomsNetwork.EventHandlers.Domain;

namespace EndToEndApiLevelTests.Scenario_Cease_a_Mobile
{
    [Collection("Scenario Cease_a_Mobile collection")]
    public class Step_1_Cease_a_Mobile
    {
        private readonly Scenario_Cease_a_Mobile_Script testScript;

        public Step_1_Cease_a_Mobile(Scenario_Cease_a_Mobile_Script testScript)
        {
            this.testScript = testScript;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = testScript.Step_1_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.ProcessingCease);
            var expectedOrderMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Order.State>(MobileOrderer.Api.Domain.Order.State.Sent);

            // Check Mobile has been updated
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check Mobile Order has been updated
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.State.Should().Be(expectedOrderMobileState);

            // Check MobileTelecomsNetwork Order has been updated
            snapshot.ActualMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be(OrderStatus.Sent);
            
            // Check ExternalMobileTelecomsNetwork Order has been created
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Type.Should().Be("Cease");
        }
    }
}
