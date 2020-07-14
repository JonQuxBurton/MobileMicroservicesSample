﻿using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Cancel_A_Mobile
{
    [Collection("Scenario Cancel_A_Mobile collection")]
    public class Step_1_Cancel_A_Mobile
    {
        private readonly Scenario_Cancel_a_Mobile_Fixture scenariosFixture;

        public Step_1_Cancel_A_Mobile(Scenario_Cancel_a_Mobile_Fixture scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = scenariosFixture.Step_1_Snapshot;
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
            snapshot.ActualMobileTelecomsNetworkOrder.Status.Should().Be("Sent");
            
            // Check ExternalMobileTelecomsNetwork Order has been created
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Should().NotBeNull();
            snapshot.ActualExternalMobileTelecomsNetworkOrder.Type.Should().Be("Cease");
        }
    }
}
