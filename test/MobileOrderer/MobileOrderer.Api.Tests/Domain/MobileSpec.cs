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
                var sut = new Mobile(Mobile.State.New, Guid.NewGuid(), 101, null, null);

                sut.CurrentState.Should().Be(Mobile.State.New);
            }
            
            [Fact]
            public void CreateMobile()
            {
                var expectedGlobalId = Guid.NewGuid();
                var expectedId = 101;
                var sut = new Mobile(Mobile.State.New, expectedGlobalId, expectedId, null, null);

                sut.GlobalId.Should().Be(expectedGlobalId);
                sut.Id.Should().Be(expectedId);
            }
        }

        public class ProvisionShould
        {
            [Fact]
            public void ChangeCurrentStateToPendingLive()
            {
                var sut = new Mobile(Mobile.State.New, Guid.NewGuid(), 101, null, null);
                
                sut.Provision();

                sut.CurrentState.Should().Be(Mobile.State.PendingLive);
            }            
            
            [Fact]
            public void ChangeInFlightOrderStateToPending()
            {
                var mobileOrder = new MobileOrder(Guid.NewGuid(), "Name", "0123456789", "New");
                var sut = new Mobile(Mobile.State.New, Guid.NewGuid(), 101, mobileOrder, null);
                
                sut.Provision();

                sut.InFlightOrder.Status.Should().Be("Pending");
            }
        }
    }
}
