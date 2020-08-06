using FluentAssertions;
using Xunit;
using Utils.Enums;

namespace EndToEndApiLevelTests.Scenario_Order_a_Mobile
{
    [Collection("Scenario Order_a_Mobile collection")]
    public class Step_1_Order_a_Mobile
    {
        private readonly Scenario_Order_a_Mobile_Script fixture;

        public Step_1_Order_a_Mobile(Scenario_Order_a_Mobile_Script fixture)
        {
            this.fixture = fixture;
        }

        [Trait("Category", "EndToEnd")]
        [Fact]
        public void Execute()
        {
            var snapshot = fixture.Step_1_Snapshot;

            // Check Customer has been created
            snapshot.ActualCustomer.Should().NotBeNull();
            snapshot.ActualCustomer.Name.Should().Be(snapshot.CustomerToAdd.Name);
        }
    }
}
