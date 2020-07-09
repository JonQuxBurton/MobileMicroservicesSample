﻿using FluentAssertions;
using Xunit;
using Utils.Enums;

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
            var snapshot = scenariosFixture.Scenario4_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.Live);
            var expectedMobileOrderState = enumConverter.ToName<MobileOrderer.Api.Domain.Order.State>(MobileOrderer.Api.Domain.Order.State.Completed);

            // Check the Mobile has been updated to Live
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);

            // Check the Activation Order has updated to Completed
            snapshot.ActualMobileActivateOrderSnapshot.Should().NotBeNull();
            snapshot.ActualMobileActivateOrderSnapshot.State.Should().Be(expectedMobileOrderState);

            // Check the MobileTelecomsNetwork Order has updated to Completed
            snapshot.ActualMobileTelecomsNetworkOrderSnapshot.Should().NotBeNull();
            snapshot.ActualMobileTelecomsNetworkOrderSnapshot.Status.Should().Be("Completed");
        }
    }
}
