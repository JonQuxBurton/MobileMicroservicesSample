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
            public void ChangeStateToPending()
            {
                var sut = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Neil", ContactPhoneNumber = "0123456789", Status = "New" });

                sut.Process();

                sut.Status.Should().Be("Pending");
            }
        }
    }
}
