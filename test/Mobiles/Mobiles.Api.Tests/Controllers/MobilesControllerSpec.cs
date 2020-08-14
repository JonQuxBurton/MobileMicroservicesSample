﻿using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mobiles.Api.Configuration;
using Mobiles.Api.Controllers;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Tests.Controllers
{
   namespace MobilesControllerSpec
    {
        public class GetShould
        {
            public GetShould()
            {
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                var loggingMock = new Mock<ILogger<MobilesController>>();

                sut = new MobilesController(loggingMock.Object, mobileRepositoryMock.Object, guidCreatorMock.Object, monitoringMock.Object);

                expectedMobile = new Mobile(
                    new MobileDataEntity {
                        Id = 101,
                        GlobalId = Guid.NewGuid(),
                        CustomerId = Guid.NewGuid(),
                        State = "New"
                    }, null, null);

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobile.GlobalId))
                    .Returns(expectedMobile);
            }

            private readonly MobilesController sut;
            private readonly Mock<IOptions<Config>> optionsMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
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
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedGlobalId;
            private readonly ActivateRequest expectedActivateRequest;

            public ActivateShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    State = State.WaitingForActivate.ToString()
                }, null);
                expectedGlobalId = Guid.NewGuid();
                expectedActivateRequest = new ActivateRequest()
                {
                    ActivationCode = "BAS132"
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();

                mobileRepositoryMock.Setup(x => x.GetById(expectedGlobalId))
                    .Returns(expectedMobile);
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);
                var loggerMock = new Mock<ILogger<MobilesController>>();

                sut = new MobilesController(loggerMock.Object, mobileRepositoryMock.Object, guidCreatorMock.Object, monitoringMock.Object);
            }

            [Fact]
            public void ActivateTheMobile()
            {
                sut.Activate(expectedGlobalId, expectedActivateRequest);

                expectedMobile.CurrentState.Should().Be(State.ProcessingActivate);
                expectedMobile.InFlightOrder.GlobalId.Should().Be(expectedGlobalId);
                expectedMobile.InFlightOrder.ActivationCode.Should().Be(expectedActivateRequest.ActivationCode);
            }

            [Fact]
            public void UpdatesTheMobileInTheRepository()
            {
                sut.Activate(expectedGlobalId, expectedActivateRequest);

                this.mobileRepositoryMock.Verify(x => x.Update(expectedMobile));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Activate(expectedGlobalId, new ActivateRequest());

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();
                mobileRepositoryMock.Setup(x => x.GetById(notFoundGlobalId))
                    .Returns((Mobile)null);

                guidCreatorMock.Setup(x => x.Create()).Returns(notFoundGlobalId);

                var actual = sut.Activate(notFoundGlobalId, new ActivateRequest());

                actual.Should().BeOfType<NotFoundResult>();
            }
        }

        public class CeaseShould
        {
            private readonly MobilesController sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedReference;

            public CeaseShould()
            {
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = "Live"
                }, null);
                expectedReference = Guid.NewGuid();

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();

                mobileRepositoryMock.Setup(x => x.GetById(expectedReference))
                    .Returns(expectedMobile);
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedReference);
                var loggerMock = new Mock<ILogger<MobilesController>>();

                sut = new MobilesController(loggerMock.Object, mobileRepositoryMock.Object, guidCreatorMock.Object, monitoringMock.Object);
            }

            [Fact]
            public void UpdateTheMobileInTheRepository()
            {
                sut.Cease(expectedReference);

                this.mobileRepositoryMock.Verify(
                    x => x.Update(It.Is<Mobile>(y =>
                        y.CurrentState == State.ProcessingCease)));
            }

            [Fact]
            public void AddInFligthOrderToRepository()
            {
                sut.Cease(expectedReference);

                mobileRepositoryMock.Verify(x => x.Update(It.Is<Mobile>(y =>
                    y.GlobalId == expectedMobile.GlobalId &&
                    y.InFlightOrder != null &&
                    y.InFlightOrder.Type == Order.OrderType.Cease &&
                    y.InFlightOrder.CurrentState == Order.State.New
                    )));
            }

            [Fact]
            public void ReturnAccepted()
            {
                var actual = sut.Cease(expectedReference);

                actual.Should().BeOfType<AcceptedResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundReference = Guid.NewGuid();
                mobileRepositoryMock.Setup(x => x.GetById(notFoundReference))
                    .Returns((Mobile)null);

                guidCreatorMock.Setup(x => x.Create()).Returns(notFoundReference);

                var actual = sut.Cease(notFoundReference);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}