using FluentAssertions;
using MobileOrderer.Api.Domain;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Domain
{
    public class OrderSpec
    {
        public class ProcessShould
        {
            [Fact]
            public void ChangeStateToProcessing()
            {
                var sut = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", State = "New" });

                sut.Process();

                sut.CurrentState.Should().Be(Order.State.Processing);
            }
        }
    }
}
