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
using static MobileOrderer.Api.Domain.Mobile;

namespace MobileOrderer.Api.Tests.Controllers
{
    public class MobilesControllerSpec
    {
        public class GetShould
        {
            public GetShould()
            {
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();

                sut = new MobilesController(mobileRepositoryMock.Object, guidCreatorMock.Object);

                expectedMobile = new Mobile(new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "New" }, null, null);

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobile.GlobalId))
                    .Returns(expectedMobile);
            }

            private readonly MobilesController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mobile expectedMobile;

            [Fact]
            public void ReturnMobile()
            {
                var actual = sut.Get(expectedMobile.GlobalId);

                var actualResult = actual.Result as OkObjectResult;
                actualResult.Value.Should().Be(expectedMobile);
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

        public class ActivateShould
        {
            private readonly MobilesController sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedGlobalId;
            private readonly OrderToAdd expectedOrder;

            public ActivateShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = "WaitingForActivation"
                }, null);
                expectedGlobalId = Guid.NewGuid();
                expectedOrder = new OrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "0123456789"
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();

                mobileRepositoryMock.Setup(x => x.GetById(expectedGlobalId))
                    .Returns(expectedMobile);
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);

                sut = new MobilesController(mobileRepositoryMock.Object, guidCreatorMock.Object);
            }

            [Fact]
            public void ActivateTheMobile()
            {
                sut.Post(expectedGlobalId, expectedOrder);

                expectedMobile.CurrentState.Should().Be(State.ProcessingActivation);
                expectedMobile.InFlightOrder.GlobalId.Should().Be(expectedGlobalId);
                expectedMobile.InFlightOrder.Name.Should().Be(expectedOrder.Name);
                expectedMobile.InFlightOrder.ContactPhoneNumber.Should().Be(expectedOrder.ContactPhoneNumber);
            }

            [Fact]
            public void UpdatesTheMobileInTheRepository()
            {
                sut.Post(expectedGlobalId, expectedOrder);

                this.mobileRepositoryMock.Verify(x => x.Update(expectedMobile));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Post(expectedGlobalId , new OrderToAdd());

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();
                mobileRepositoryMock.Setup(x => x.GetById(notFoundGlobalId))
                    .Returns((Mobile)null);

                guidCreatorMock.Setup(x => x.Create()).Returns(notFoundGlobalId);

                var actual = sut.Post(notFoundGlobalId, new OrderToAdd());

                actual.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}
