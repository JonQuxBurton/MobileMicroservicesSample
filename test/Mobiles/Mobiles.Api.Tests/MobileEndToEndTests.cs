using FluentAssertions;
using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Utils.DateTimes;
using Xunit;

namespace Mobiles.Api.Tests
{
    public class MobileEndToEndTests
    {
        [Fact]
        public void ProvisionToWaitingForActivateScenario()
        {
            var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

            var provisionOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
            {
                Id = 101, GlobalId = Guid.NewGuid(), State = "New",
                CreatedAt = DateTime.MinValue,
                Orders = new List<OrderDataEntity>() { provisionOrder.GetDataEntity() }
            });

            sut.State.Should().Be(Mobile.MobileState.New);

            sut.Provision();

            sut.State.Should().Be(Mobile.MobileState.ProcessingProvision);
            sut.InProgressOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InProgressOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InProgressOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ProcessingProvisionCompleted();

            sut.State.Should().Be(Mobile.MobileState.WaitingForActivate);
            sut.InProgressOrder.Should().BeNull();
            sut.Orders.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }

        [Fact]
        public void WaitingForActivateToLiveScenario()
        {
            var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

            var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
            {
                Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.WaitingForActivate.ToString()
            });

            var activateOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
            sut.Activate(activateOrder);

            sut.State.Should().Be(Mobile.MobileState.ProcessingActivate);
            sut.InProgressOrder.CurrentState.Should().Be(Api.Domain.Order.State.New);

            sut.OrderProcessing();

            sut.InProgressOrder.CurrentState.Should().Be(Api.Domain.Order.State.Processing);

            sut.OrderSent();

            sut.InProgressOrder.CurrentState.Should().Be(Api.Domain.Order.State.Sent);

            sut.ActivateCompleted();

            sut.State.Should().Be(Mobile.MobileState.Live);
            sut.InProgressOrder.Should().BeNull();
            sut.Orders.First().CurrentState.Should().Be(Api.Domain.Order.State.Completed);
        }
    }
}
