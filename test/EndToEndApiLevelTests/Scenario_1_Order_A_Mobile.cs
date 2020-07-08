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

            // Check Mobiles database
            var expectedOrder = scenariosFixture.Scenario1OrderToAdd;
            var actualMobile = scenariosFixture.Scenario1ActualMobileSnapshot;
            actualMobile.Should().NotBeNull();
            actualMobile.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Mobile.State), MobileOrderer.Api.Domain.Mobile.State.ProcessingProvisioning));
            var actualMobileOrder = scenariosFixture.Scenario1ActualMobileOrderSnapshot;
            actualMobileOrder.Should().NotBeNull();
            actualMobileOrder.Name.Should().Be(expectedOrder.Name);
            actualMobileOrder.ContactPhoneNumber.Should().Be(expectedOrder.ContactPhoneNumber);
            actualMobileOrder.State.Should().Be(Enum.GetName(typeof(MobileOrderer.Api.Domain.Order.State), MobileOrderer.Api.Domain.Order.State.Sent));

            // Check SIM Cards database
            var actualSimCardOrder = scenariosFixture.Scenario1ActualSimCardOrder;
            actualSimCardOrder.Should().NotBeNull();
            actualSimCardOrder.Name.Should().Be(expectedOrder.Name);

            // Check External SIM Card system
            scenariosFixture.Scenario1ExternalSimCardOrder.Should().NotBeNull();
        }
    }
}
