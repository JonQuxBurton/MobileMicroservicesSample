using FluentAssertions;
using MobileOrderer.Api.Domain;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests.Domain
{
    public class MobileOrderSpec
    {
        public class ProcessShould
        {
            [Fact]
            public void ChangeStateToPending()
            {
                var sut = new MobileOrder(Guid.NewGuid(), "Neil", "0123456789", "New");

                sut.Process();

                sut.Status.Should().Be("Pending");
            }
        }
    }
}
