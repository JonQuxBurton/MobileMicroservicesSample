using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MobileOrderer.Api.Configuration;
using MobileOrderer.Api.Controllers;
using MobileOrderer.Api.Domain;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;

namespace MobileOrderer.Api.Tests
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
