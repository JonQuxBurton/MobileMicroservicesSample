using FluentAssertions;
using MobileOrderer.Api.Domain;
using System;
using System.Linq;
using Xunit;

namespace MobileOrderer.Api.Tests.Domain
{
    public class MobileSpec
    {
        public class ConstructorShould
        {
            [Fact]
            public void CreateMobileInStateNew()
            {
                var sut = new Mobile(new MobileDataEntity{ Id = 101, GlobalId = Guid.NewGuid(), State = "New" }, null, null);

                sut.CurrentState.Should().Be(Mobile.State.New);
            }

            [Fact]
            public void CreateMobile()
            {
                var expectedGlobalId = Guid.NewGuid();
                var expectedId = 101;
                var sut = new Mobile(new MobileDataEntity {Id = expectedId, GlobalId = expectedGlobalId, State = "New" }, null, null);

                sut.GlobalId.Should().Be(expectedGlobalId);
                sut.Id.Should().Be(expectedId);
            }
        }

        public class ProvisionShould
        {
            [Fact]
            public void ChangeCurrentStateToPendingActivation()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid() , State = "New"}, null, null);

                sut.Provision(mobileOrder);

                sut.CurrentState.Should().Be(Mobile.State.ProcessingProvisioning);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid() , State = "New"}, mobileOrder, null);

                sut.Provision(mobileOrder);

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }

        public class ActivateShould
        {
            [Fact]
            public void ChangeCurrentStateToProcessingActivation()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "WaitingForActivation" }, null, null);

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.CurrentState.Should().Be(Mobile.State.ProcessingActivation);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "WaitingForActivation" }, mobileOrder, null);

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }
        
        public class ActivationCompletedShould
        {
            [Fact]
            public void ChangeCurrentStateToLive()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "ProcessingActivation" }, null, null);

                sut.ActivateCompleted();

                sut.CurrentState.Should().Be(Mobile.State.Live);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "ProcessingActivation" }, mobileOrder, null);

                sut.ActivateCompleted();

                sut.OrderHistory.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInFlightOrderToNull()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "ProcessingActivation" }, mobileOrder, null);

                sut.ActivateCompleted();

                sut.InFlightOrder.Should().BeNull();
            }
        }

        public class OrderProcessingShould
        {
            [Theory]
            [InlineData("PendingActivation")]
            [InlineData("PendingLive")]
            public void SetInFlightOrderStateToProcessing(string mobileState)
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = mobileState }, mobileOrder, null);

                sut.OrderProcessing();

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.Processing);
            }
        }

        public class OrderSentShould
        {
            [Theory]
            [InlineData("PendingActivation")]
            [InlineData("PendingLive")]
            public void SetInFlightOrderStateToSent(string mobileState)
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Processing" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = mobileState }, mobileOrder, null);

                sut.OrderSent();

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.Sent);
            }
        }
    }
}
