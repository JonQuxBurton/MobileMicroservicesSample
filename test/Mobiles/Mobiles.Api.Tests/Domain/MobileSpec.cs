using FluentAssertions;
using Mobiles.Api.Domain;
using System;
using System.Linq;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace MobileSpec
    {
        public class ConstructorShould
        {
            [Fact]
            public void CreateMobileInStateNew()
            {
                var sut = new Mobile(new MobileDataEntity{ Id = 101, GlobalId = Guid.NewGuid(), State = "New" }, null, null);

                sut.State.Should().Be(Mobile.MobileState.New);
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

                sut.State.Should().Be(Mobile.MobileState.ProcessingProvision);
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
            public void ChangeCurrentStateToProcessingActivate()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.WaitingForActivate.ToString() }, null, null);

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.State.Should().Be(Mobile.MobileState.ProcessingActivate);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.WaitingForActivate.ToString() }, mobileOrder, null);

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }
        
        public class ActivateCompletedShould
        {
            [Fact]
            public void ChangeCurrentStateToLive()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingActivate.ToString() }, null, null);

                sut.ActivateCompleted();

                sut.State.Should().Be(Mobile.MobileState.Live);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingActivate.ToString() }, mobileOrder, null);

                sut.ActivateCompleted();

                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInFlightOrderToNull()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingActivate.ToString() }, mobileOrder, null);

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

        public class CeaseShould
        {
            [Fact]
            public void ChangeCurrentStateToProcessingCease()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "Live" }, null, null);

                sut.Cease(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.State.Should().Be(Mobile.MobileState.ProcessingCease);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "Live" }, mobileOrder, null);

                sut.Cease(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }

        public class CeaseCompletedShould
        {
            [Fact]
            public void ChangeCurrentStateToCeased()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingCease.ToString() }, null, null);

                sut.CeaseCompleted();

                sut.State.Should().Be(Mobile.MobileState.Ceased);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingCease.ToString() }, mobileOrder, null);

                sut.CeaseCompleted();

                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInFlightOrderToNull()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingCease.ToString() }, mobileOrder, null);

                sut.CeaseCompleted();

                sut.InFlightOrder.Should().BeNull();
            }
        }
    }
}
