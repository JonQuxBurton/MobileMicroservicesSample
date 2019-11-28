using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Controllers;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using Moq;
using System;
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
            public void SaveMobileInRepositoryWithStateOfNew()
            {
                var expectedOrder = new MobileOrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };
                var expectedGlobalId = Guid.NewGuid();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);

                sut.Post(expectedOrder);

                mobileRepositoryMock.Verify(x => x.Save(It.Is<Mobile>(y =>
                    y.GlobalId == expectedGlobalId &&
                    y.CurrentState == Mobile.State.New)));
            }            
            
            [Fact]
            public void SaveMobileInRepositoryWithInFligthOrder()
            {
                var expectedOrder = new MobileOrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };
                var expectedGlobalId = Guid.NewGuid();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);

                sut.Post(expectedOrder);

                mobileRepositoryMock.Verify(x => x.Save(It.Is<Mobile>(y =>
                    y.GlobalId == expectedGlobalId &&
                    y.CurrentState == Mobile.State.New &&
                    y.InFlightOrder != null &&
                    y.InFlightOrder.Status == "New")));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Post(new MobileOrderToAdd());

                actual.Should().BeOfType<OkResult>();
            }
        }

        public class GetShould
        {
            public GetShould()
            {
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();

                sut = new ProvisionerController(mobileRepositoryMock.Object, guidCreatorMock.Object);

                expectedMobile = new Mobile(Mobile.State.New, Guid.NewGuid(), 101, null, null);

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobile.GlobalId))
                    .Returns(expectedMobile);
            }

            private readonly ProvisionerController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mobile expectedMobile;

            [Fact]
            public void ReturnMobile()
            {
                var actual = sut.Get(expectedMobile.GlobalId);
                
                var actualResult = actual.Result as OkObjectResult;
                actualResult.Value.Should().Be(this.expectedMobile);
            }
            
            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Get(expectedMobile.GlobalId);
                
                actual.Result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var actual = sut.Get(Guid.NewGuid());
                
                actual.Result.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}