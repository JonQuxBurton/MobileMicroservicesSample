using FluentAssertions;
using Xunit;
using System;

namespace EndToEndApiLevelTests
{
    [Collection("Scenarios collection")]
    public class Scenario_1_Order_A_Mobile
    {
        private readonly ScenariosFixture scenariosFixture;

        public Scenario_1_Order_A_Mobile(ScenariosFixture scenariosFixture)
        {
            this.scenariosFixture = scenariosFixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = scenariosFixture.scenario1_Snapshot;

            // Check Mobiles database
            var expectedOrder = snapshot.OrderToAdd;
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.ProcessingProvisioning));
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.Name.Should().Be(expectedOrder.Name);
            snapshot.ActualMobileOrder.ContactPhoneNumber.Should().Be(expectedOrder.ContactPhoneNumber);
            snapshot.ActualMobileOrder.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Order.State), MobileOrderer.Api.Domain.Order.State.Sent));

            // Check SIM Cards database
            snapshot.ActualSimCardOrder.Should().NotBeNull();
            snapshot.ActualSimCardOrder.Name.Should().Be(expectedOrder.Name);

            // Check External SIM Card system
            snapshot.ActualExternalSimCardOrder.Should().NotBeNull();
        }
    }
}
