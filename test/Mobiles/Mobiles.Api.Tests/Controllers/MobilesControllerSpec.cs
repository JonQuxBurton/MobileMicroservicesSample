using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Mobiles.Api.Configuration;
using Mobiles.Api.Controllers;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using Moq;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Xunit;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Tests.Controllers
{
    namespace MobilesControllerSpec
    {
        public class GetShould
        {
            private readonly Order expectedInProgressOrder;
            private readonly Mobile expectedMobile;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMobilesService> mobilesServiceMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Mock<IOptions<Config>> optionsMock;

            private readonly MobilesController sut;

            public GetShould()
            {
                mobilesServiceMock = new Mock<IMobilesService>();
                optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                monitoringMock = new Mock<IMonitoring>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                sut = new MobilesController(mobilesServiceMock.Object, mobileRepositoryMock.Object, monitoringMock.Object);

                expectedInProgressOrder = new Order(new OrderDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    State = Order.State.New.ToString()
                });
                expectedMobile = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101,
                        GlobalId = Guid.NewGuid(),
                        CustomerId = Guid.NewGuid(),
                        State = MobileState.New.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            expectedInProgressOrder.GetDataEntity()
                        }
                    });

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobile.GlobalId))
                    .Returns(expectedMobile);
            }

            [Fact]
            public void ReturnExpectedMobile()
            {
                var actual = sut.Get(expectedMobile.GlobalId);

                var actualResult = actual.Result as OkObjectResult;
                var actualMobileResource = actualResult.Value as MobileResource;
                actualMobileResource.GlobalId.Should().Be(expectedMobile.GlobalId);
                actualMobileResource.CustomerId.Should().Be(expectedMobile.CustomerId);
                actualMobileResource.CreatedAt.Should().Be(expectedMobile.CreatedAt);
                actualMobileResource.Orders.First().State.Should().Be(expectedInProgressOrder.CurrentState.ToString());
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
        
        public class GetAvailablePhoneNumbersShould
        {
            private readonly Order expectedInProgressOrder;
            private readonly Mobile expectedMobile;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMobilesService> mobilesServiceMock;
            private readonly Mock<IMonitoring> monitoringMock;

            private readonly MobilesController sut;
            private readonly string expectedAvailablePhoneNumber;

            public GetAvailablePhoneNumbersShould()
            {
                mobilesServiceMock = new Mock<IMobilesService>();
                var optionsMock = new Mock<IOptions<Config>>();
                optionsMock.Setup(x => x.Value).Returns(new Config());
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                monitoringMock = new Mock<IMonitoring>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expectedInProgressOrder = new Order(new OrderDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    State = Order.State.New.ToString()
                });
                expectedMobile = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101,
                        GlobalId = Guid.NewGuid(),
                        CustomerId = Guid.NewGuid(),
                        State = MobileState.New.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            expectedInProgressOrder.GetDataEntity()
                        }
                    });
                expectedAvailablePhoneNumber = "07101000101";
                mobilesServiceMock.Setup(x => x.GetAvailablePhoneNumbers())
                    .Returns(new [] {expectedAvailablePhoneNumber });
                mobileRepositoryMock.Setup(x => x.GetById(expectedMobile.GlobalId))
                    .Returns(expectedMobile);

                sut = new MobilesController(mobilesServiceMock.Object, mobileRepositoryMock.Object, monitoringMock.Object);
            }

            [Fact]
            public void ReturnAvailablePhoneNumbers()
            {
                var actual = sut.GetAvailablePhoneNumbers();

                actual.Value.PhoneNumbers.First().Should().Be(expectedAvailablePhoneNumber);
            }
        }

        public class ActivateShould
        {
            private readonly ActivateRequest expectedActivateRequest;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedMobileGlobalId;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMobilesService> mobilesServiceMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly MobilesController sut;
            private readonly OrderDataEntity expectedOrderDataEntity;
            private readonly Guid expectedOrderGlobalId;

            public ActivateShould()
            {
                mobilesServiceMock = new Mock<IMobilesService>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expectedMobileGlobalId = Guid.NewGuid();
                expectedOrderGlobalId = Guid.NewGuid();
                expectedActivateRequest = new ActivateRequest
                {
                    ActivationCode = "BAS132"
                };
                expectedOrderDataEntity = new OrderDataEntity
                {
                    GlobalId = expectedOrderGlobalId,
                    Type = Order.OrderType.Activate.ToString(),
                    State = Order.State.New.ToString(),
                    ActivationCode = expectedActivateRequest.ActivationCode,
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "0700123456",
                    CreatedAt = new DateTime(2001, 5, 4),
                    UpdatedAt = new DateTime(2002, 6, 5)
                };
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    State = MobileState.ProcessingActivate.ToString(),
                    Orders = new List<OrderDataEntity>
                    {
                        expectedOrderDataEntity
                    }
                });

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                monitoringMock = new Mock<IMonitoring>();

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobileGlobalId))
                    .Returns(expectedMobile);
                mobilesServiceMock.Setup(x => x.Activate(expectedMobileGlobalId, expectedActivateRequest))
                    .Returns(expectedMobile);

                sut = new MobilesController(mobilesServiceMock.Object, mobileRepositoryMock.Object,
                    monitoringMock.Object);
            }

            [Fact]
            public void SendActivateRequestToMobilesService()
            {
                sut.Activate(expectedMobileGlobalId, expectedActivateRequest);

                mobilesServiceMock.Verify(x => x.Activate(expectedMobileGlobalId, expectedActivateRequest));
            }

            [Fact]
            public void ReturnNewOrder()
            {
                var actual = sut.Activate(expectedMobileGlobalId, expectedActivateRequest);

                var actualResult = actual as OkObjectResult;
                var actualOrder = actualResult.Value as OrderResource;

                actualOrder.Should().NotBeNull();
                actualOrder.GlobalId.Should().Be(expectedOrderGlobalId);
                actualOrder.Name.Should().Be(expectedOrderDataEntity.Name);
                actualOrder.ContactPhoneNumber.Should().Be(expectedOrderDataEntity.ContactPhoneNumber);
                actualOrder.State.Should().Be(expectedOrderDataEntity.State);
                actualOrder.Type.Should().Be(expectedOrderDataEntity.Type);
                actualOrder.CreatedAt.Should().Be(expectedOrderDataEntity.CreatedAt);
                actualOrder.UpdatedAt.Should().Be(expectedOrderDataEntity.UpdatedAt);
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Activate(expectedMobileGlobalId, expectedActivateRequest);

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();
                var activateRequest = new ActivateRequest();
                mobilesServiceMock.Setup(x => x.Activate(notFoundGlobalId, activateRequest))
                    .Returns(null as Mobile);

                var actual = sut.Activate(notFoundGlobalId, activateRequest);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }

        public class CeaseShould
        {
            private readonly Mobile expectedMobile;
            private readonly Guid expectedMobileGlobalId;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMobilesService> mobilesServiceMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly MobilesController sut;

            public CeaseShould()
            {
                mobilesServiceMock = new Mock<IMobilesService>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    State = "Live"
                });
                expectedMobileGlobalId = Guid.NewGuid();

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                monitoringMock = new Mock<IMonitoring>();
                mobileRepositoryMock.Setup(x => x.GetById(expectedMobileGlobalId))
                    .Returns(expectedMobile);
                mobilesServiceMock.Setup(x => x.Cease(expectedMobileGlobalId))
                    .Returns(expectedMobile);

                sut = new MobilesController(mobilesServiceMock.Object, mobileRepositoryMock.Object,
                    monitoringMock.Object);
            }

            [Fact]
            public void SendCeaseToMobilesService()
            {
                sut.Cease(expectedMobileGlobalId);

                mobilesServiceMock.Verify(x => x.Cease(expectedMobileGlobalId));
            }

            [Fact]
            public void ReturnAccepted()
            {
                var actual = sut.Cease(expectedMobileGlobalId);

                actual.Should().BeOfType<AcceptedResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundMobileGlobalId = Guid.NewGuid();
                mobilesServiceMock.Setup(x => x.Cease(notFoundMobileGlobalId)).Returns(null as Mobile);

                var actual = sut.Cease(notFoundMobileGlobalId);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}