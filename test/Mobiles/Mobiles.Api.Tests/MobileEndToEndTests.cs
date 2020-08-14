using FluentAssertions;
using Mobiles.Api.Domain;
using System;
using System.Linq;
using Xunit;

namespace Mobiles.Api.Tests
{
    public class MobileEndToEndTests
    {
        [Fact]
        public void ProvisionToWaitingForActivateScenario()
        {
            var provisionOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "New" }, provisionOrder, null);

            sut.CurrentState.Should().Be(Mobile.State.New);

            sut.Provision(provisionOrder);

            sut.CurrentState.Should().Be(Mobile.State.ProcessingProvision);
            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ProcessingProvisionCompleted();

            sut.CurrentState.Should().Be(Mobile.State.WaitingForActivate);
            sut.InFlightOrder.Should().BeNull();
            sut.OrderHistory.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }

        [Fact]
        public void WaitingForActivateToLiveScenario()
        {
            var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.State.WaitingForActivate.ToString() }, null, null);

            var activateOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            sut.Activate(activateOrder);

            sut.CurrentState.Should().Be(Mobile.State.ProcessingActivate);
            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ActivateCompleted();

            sut.CurrentState.Should().Be(Mobile.State.Live);
            sut.InFlightOrder.Should().BeNull();
            sut.OrderHistory.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }
    }
}
