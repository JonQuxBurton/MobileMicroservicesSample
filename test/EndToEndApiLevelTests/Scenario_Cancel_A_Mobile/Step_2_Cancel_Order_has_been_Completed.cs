using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Cancel_A_Mobile
{
    [Collection("Scenario Cancel_A_Mobile collection")]
    public class Step_2_Cancel_Order_has_been_Completed
    {
        private readonly Scenario_Cancel_A_Mobile_Fixture scenariosFixture;

        public Step_2_Cancel_Order_has_been_Completed(Scenario_Cancel_A_Mobile_Fixture scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = scenariosFixture.Step_2_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.Ceased);
            var expectedOrderMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Order.State>(MobileOrderer.Api.Domain.Order.State.Completed);

            // Check Mobile has been updated
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check Mobile Order has been updated
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.State.Should().Be(expectedOrderMobileState);

            // Check MobileTelecomsNetwork Order has been updated
            snapshot.ActualMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be("Completed");
        }
    }
}
