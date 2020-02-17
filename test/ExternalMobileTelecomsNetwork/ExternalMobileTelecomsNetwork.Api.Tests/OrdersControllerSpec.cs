using ExternalMobileTelecomsNetwork.Api.Controllers;
using ExternalMobileTelecomsNetwork.Api.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace ExternalMobileTelecomsNetwork.Api.Tests
{
    public class OrdersControllerSpec
    {
        public class StatusShould
        {
            [Fact]
            public void ReturnOk()
            {
                var dataStoreMock = new Mock<IDataStore>();
                var sut = new OrdersController(dataStoreMock.Object);
                var actual = sut.Status();

                actual.Should().BeOfType<OkResult>();
            }
        }
    }
}
