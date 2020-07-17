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
                monitoringMock = new Mock<IMonitoring>();

                var mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                sut = new ProvisionerController(mobileRepositoryMock.Object, guidCreatorMock.Object, monitoringMock.Object);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;

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
                monitoringMock = new Mock<IMonitoring>();

                expectedGlobalId = Guid.NewGuid();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);

                sut = new ProvisionerController(mobileRepositoryMock.Object, guidCreatorMock.Object, monitoringMock.Object);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Guid expectedGlobalId;

            [Fact]
            public void AddMobileToRepositoryWithStateOfNew()
            {
                var expectedOrder = new OrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };

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

                actual.Should().BeOfType<OkObjectResult>();
            }
            
            [Fact]
            public void ReturnNewMobile()
            {
                var actual = sut.Post(new OrderToAdd());
                var actualResult = actual as OkObjectResult;
                var actualMobile = actualResult.Value as MobileResource;

                actualMobile.Should().NotBeNull();
                actualMobile.GlobalId.Should().Be(expectedGlobalId);
            }
        }
    }
}