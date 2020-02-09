using FluentAssertions;
using MobileOrderer.Api.Domain;
using System;
using System.Linq;
using Xunit;

namespace MobileOrderer.Api.Tests
{
    public class MobileEndToEndTests
    {
        [Fact]
        public void ProvisionToWaitingForActivationScenario()
        {
            var provisionOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "New" }, null, null);

            sut.CurrentState.Should().Be(Mobile.State.New);

            sut.Provision(provisionOrder);

            sut.CurrentState.Should().Be(Mobile.State.ProcessingProvisioning);
            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ProcessingProvisioningCompleted();

            sut.CurrentState.Should().Be(Mobile.State.WaitingForActivation);
            sut.InFlightOrder.Should().BeNull();
            sut.OrderHistory.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }

        [Fact]
        public void WaitingForActivationToLiveScenario()
        {
            var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "WaitingForActivation" }, null, null);

            var activateOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            sut.Activate(activateOrder);

            sut.CurrentState.Should().Be(Mobile.State.ProcessingActivation);
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
