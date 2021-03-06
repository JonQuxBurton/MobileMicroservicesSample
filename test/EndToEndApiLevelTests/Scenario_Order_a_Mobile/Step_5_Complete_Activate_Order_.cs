﻿using FluentAssertions;
using Xunit;
using Utils.Enums;
using MobileTelecomsNetwork.EventHandlers.Domain;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    [Collection("Scenario Order_a_Mobile collection")]
    public class Step_5_Complete_Activate_Order
    {
        private readonly Scenario_Order_a_Mobile_Script fixture;

        public Step_5_Complete_Activate_Order(Scenario_Order_a_Mobile_Script fixture)
        {
            this.fixture = fixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = fixture.Step_5_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<Mobiles.Api.Domain.Mobile.MobileState>(Mobiles.Api.Domain.Mobile.MobileState.Live);
            var expectedMobileOrderState = enumConverter.ToName<Mobiles.Api.Domain.Order.State>(Mobiles.Api.Domain.Order.State.Completed);

            // Check the Mobile has been updated to Live
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check the Activation Order has updated to Completed
            snapshot.ActualMobileActivateOrder.Should().NotBeNull();
            snapshot.ActualMobileActivateOrder.State.Should().Be(expectedMobileOrderState);

            // Check the MobileTelecomsNetwork Order has updated to Completed
            snapshot.ActualMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be(OrderStatus.Completed);
        }
    }
}
