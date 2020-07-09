using FluentAssertions;
using Xunit;
using Utils.Enums;

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
            var snapshot = scenariosFixture.Scenario1_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Mobile.State>(MobileOrderer.Api.Domain.Mobile.State.ProcessingProvisioning);
            var expectedOrderMobileState = enumConverter.ToName<MobileOrderer.Api.Domain.Order.State>(MobileOrderer.Api.Domain.Order.State.Sent);

            // Check Mobiles database
            var expectedOrder = snapshot.OrderToAdd;
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.Name.Should().Be(expectedOrder.Name);
            snapshot.ActualMobileOrder.ContactPhoneNumber.Should().Be(expectedOrder.ContactPhoneNumber);
            snapshot.ActualMobileOrder.State.Should().Be(expectedOrderMobileState);

            // Check SIM Cards database
            snapshot.ActualSimCardOrder.Should().NotBeNull();
            snapshot.ActualSimCardOrder.Name.Should().Be(expectedOrder.Name);

            // Check External SIM Card system
            snapshot.ActualExternalSimCardOrder.Should().NotBeNull();
        }
    }
}
