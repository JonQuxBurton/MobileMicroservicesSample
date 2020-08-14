using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    [Collection("Scenario Order_a_Mobile collection")]
    public class Step_2_Order_a_Mobile
    {
        private readonly Scenario_Order_a_Mobile_Script fixture;

        public Step_2_Order_a_Mobile(Scenario_Order_a_Mobile_Script fixture)
        {
            this.fixture = fixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = fixture.Step_2_Snapshot;
            var enumConverter = new EnumConverter();
            var expectedMobileState = enumConverter.ToName<Mobiles.Api.Domain.Mobile.State>(Mobiles.Api.Domain.Mobile.State.ProcessingProvision);
            var expectedOrderMobileState = enumConverter.ToName<Mobiles.Api.Domain.Order.State>(Mobiles.Api.Domain.Order.State.Sent);

            // Check Mobile has been updated
            var expectedOrder = snapshot.OrderToAdd;
            snapshot.ActualMobile.Should().NotBeNull();
            snapshot.ActualMobile.State.Should().Be(expectedMobileState);
            snapshot.ActualMobile.CustomerId.Should().Be(snapshot.ActualCustomer.GlobalId);

            // Check Mobile Order has been updated
            snapshot.ActualMobileOrder.Should().NotBeNull();
            snapshot.ActualMobileOrder.Name.Should().Be(expectedOrder.Name);
            snapshot.ActualMobileOrder.ContactPhoneNumber.Should().Be(expectedOrder.ContactPhoneNumber);
            snapshot.ActualMobileOrder.State.Should().Be(expectedOrderMobileState);

            // Check SIM Card Order has been updated
            snapshot.ActualSimCardOrder.Should().NotBeNull();
            snapshot.ActualSimCardOrder.Name.Should().Be(expectedOrder.Name);
            snapshot.ActualSimCardOrder.Status.Should().Be("Sent");

            // Check the Provisioning Order has been sent to the External SIM Card system
            snapshot.ActualExternalSimCardOrder.Should().NotBeNull();
        }
    }
}
