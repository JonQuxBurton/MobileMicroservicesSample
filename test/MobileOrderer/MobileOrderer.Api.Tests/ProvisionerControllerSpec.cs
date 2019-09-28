using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Controllers;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Resources;
using Moq;
using Xunit;

namespace MobileOrderer.Api.Tests
{
    public class ProvisionerControllerSpec
    {
        public class StatusShould
        {
            public StatusShould()
            {
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                ordersDataStoreMock = new Mock<IOrdersDataStore>();

                sut = new ProvisionerController(ordersDataStoreMock.Object);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IOrdersDataStore> ordersDataStoreMock;

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Status();

                actual.Should().BeOfType<OkResult>();
            }
        }

        public class PostShould
        {
            public PostShould()
            {
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                ordersDataStoreMock = new Mock<IOrdersDataStore>();

                sut = new ProvisionerController(ordersDataStoreMock.Object);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IOrdersDataStore> ordersDataStoreMock;

            [Fact]
            public void AddOrderToDataStoreWithStatusOfNew()
            {
                var expectedOrder = new MobileOrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };

                sut.Post(expectedOrder);

                ordersDataStoreMock.Verify(x => x.Add(It.Is<MobileOrder>(y =>
                    y.Name == expectedOrder.Name &&
                    y.ContactPhoneNumber == expectedOrder.ContactPhoneNumber &&
                    y.Status == "New")));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Post(new MobileOrderToAdd());

                actual.Should().BeOfType<OkResult>();
            }
        }
    }
}