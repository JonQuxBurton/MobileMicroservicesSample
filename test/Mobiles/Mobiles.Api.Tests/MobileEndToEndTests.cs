﻿using FluentAssertions;
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

            sut.State.Should().Be(Mobile.MobileState.New);

            sut.Provision(provisionOrder);

            sut.State.Should().Be(Mobile.MobileState.ProcessingProvision);
            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ProcessingProvisionCompleted();

            sut.State.Should().Be(Mobile.MobileState.WaitingForActivate);
            sut.InFlightOrder.Should().BeNull();
            sut.Orders.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }

        [Fact]
        public void WaitingForActivateToLiveScenario()
        {
            var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.WaitingForActivate.ToString() }, null, null);

            var activateOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            sut.Activate(activateOrder);

            sut.State.Should().Be(Mobile.MobileState.ProcessingActivate);
            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InFlightOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ActivateCompleted();

            sut.State.Should().Be(Mobile.MobileState.Live);
            sut.InFlightOrder.Should().BeNull();
            sut.Orders.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }
    }
}
