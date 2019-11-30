using FluentAssertions;
using MobileOrderer.Api.Domain;
using System;
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
            public void ChangeCurrentStateToPendingLive()
            {
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid() , State = "New"}, null, null);

                sut.Provision();

                sut.CurrentState.Should().Be(Mobile.State.PendingLive);
            }

            [Fact]
            public void ChangeInFlightOrderStateToPending()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid() , State = "New"}, mobileOrder, null);

                sut.Provision();

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.Processing);
            }
        }
    }
}
