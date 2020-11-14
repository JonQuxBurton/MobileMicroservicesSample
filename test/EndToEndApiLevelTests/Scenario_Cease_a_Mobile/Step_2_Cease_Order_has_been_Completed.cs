using FluentAssertions;
using Xunit;
using Utils.Enums;
using MobileTelecomsNetwork.EventHandlers.Domain;

namespace EndToEndApiLevelTests.Scenario_Cease_a_Mobile
{
    [Collection("Scenario Cease_a_Mobile collection")]
    public class Step_2_Cease_Order_has_been_Completed
    {
        private readonly Scenario_Cease_a_Mobile_Script scenariosFixture;

        public Step_2_Cease_Order_has_been_Completed(Scenario_Cease_a_Mobile_Script scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = scenariosFixture.Step_2_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<Mobiles.Api.Domain.Mobile.MobileState>(Mobiles.Api.Domain.Mobile.MobileState.Ceased);
            var expectedOrderMobileState = enumConverter.ToName<Mobiles.Api.Domain.Order.State>(Mobiles.Api.Domain.Order.State.Completed);

            // Check Mobile has been updated
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check Mobile Order has been updated
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.State.Should().Be(expectedOrderMobileState);

            // Check MobileTelecomsNetwork Order has been updated
            snapshot.ActualMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be(OrderStatus.Completed);
        }
    }
}
