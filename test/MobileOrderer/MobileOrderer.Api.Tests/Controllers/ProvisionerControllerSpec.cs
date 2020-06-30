using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Controllers;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;

namespace MobileOrderer.Api.Tests.Controllers
{
    public class ProvisionerControllerSpec
    {
        public class StatusShould
        {
            public StatusShould()
            {
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                guidCreatorMock = new Mock<IGuidCreator>();

                var mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                sut = new ProvisionerController(mobileRepositoryMock.Object, guidCreatorMock.Object);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;

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
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();

                sut = new ProvisionerController(mobileRepositoryMock.Object, guidCreatorMock.Object);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;

            [Fact]
            public void AddMobileToRepositoryWithStateOfNew()
            {
                var expectedOrder = new OrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };
                var expectedGlobalId = Guid.NewGuid();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);

                sut.Post(expectedOrder);

                mobileRepositoryMock.Verify(x => x.Add(It.Is<Mobile>(y =>
                    y.GlobalId == expectedGlobalId &&
                    y.CurrentState == Mobile.State.New)));
            }

            [Fact]
            public void AddMobileToRepositoryWithInFligthOrder()
            {
                var expectedOrder = new OrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };
                var expectedGlobalId = Guid.NewGuid();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);

                sut.Post(expectedOrder);

                mobileRepositoryMock.Verify(x => x.Add(It.Is<Mobile>(y =>
                    y.GlobalId == expectedGlobalId &&
                    y.CurrentState == Mobile.State.New &&
                    y.InFlightOrder != null &&
                    y.InFlightOrder.CurrentState == Order.State.New)));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Post(new OrderToAdd());

                actual.Should().BeOfType<OkResult>();
            }
        }
    }
}