using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using Moq;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace MobilesServiceSpec
    {
        public class ActivateShould
        {
            private readonly MobilesService sut;

            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;
            private readonly ActivateRequest expectedActivateRequest;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedMobileGlobalId;
            private readonly Guid expectedNewOrderGlobalId;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<ILogger<MobilesService>> loggerMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetNextMobileIdQuery> getNextMobileIdQueryMock;

            public ActivateShould()
            {
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                loggerMock = new Mock<ILogger<MobilesService>>();
                guidCreatorMock = new Mock<IGuidCreator>(); 
                getNextMobileIdQueryMock = new Mock<IGetNextMobileIdQuery>();

                expectedMobileGlobalId = Guid.NewGuid();
                expectedNewOrderGlobalId = Guid.NewGuid();
                expectedActivateRequest = new ActivateRequest
                {
                    ActivationCode = "ACT001"
                };
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    GlobalId = expectedMobileGlobalId,
                    State = Mobile.MobileState.WaitingForActivate.ToString()
                });
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedNewOrderGlobalId);

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobileGlobalId))
                    .Returns(expectedMobile);

                sut = new MobilesService(loggerMock.Object, mobileRepositoryMock.Object, guidCreatorMock.Object, getNextMobileIdQueryMock.Object);
            }

            [Fact]
            public void ReturnUpdatedMobile()
            {
                var actual = sut.Activate(expectedMobileGlobalId, expectedActivateRequest);

                actual.GlobalId.Should().Be(expectedMobileGlobalId);
                actual.InProgressOrder.GlobalId.Should().Be(expectedNewOrderGlobalId);
                actual.InProgressOrder.ActivationCode.Should().Be(expectedActivateRequest.ActivationCode);
                actual.InProgressOrder.CurrentState.Should().Be(Order.State.New);
                actual.InProgressOrder.Type.Should().Be(Order.OrderType.Activate);
            }

            [Fact]
            public void UpdateMobileInRepository()
            {
                sut.Activate(expectedMobileGlobalId, expectedActivateRequest);

                mobileRepositoryMock.Verify(x => x.Update(It.Is<Mobile>(y =>
                    y.GlobalId == expectedMobileGlobalId &&
                    y.State == Mobile.MobileState.ProcessingActivate &&
                    y.InProgressOrder != null &&
                    y.InProgressOrder.CurrentState == Order.State.New &&
                    y.InProgressOrder.ActivationCode == expectedActivateRequest.ActivationCode &&
                    y.InProgressOrder.Type == Order.OrderType.Activate)));
            }
        }

        public class CeaseShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedMobileGlobalId;
            private readonly Guid expectedNewOrderGlobalId;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<ILogger<MobilesService>> loggerMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;

            private readonly MobilesService sut;
            private readonly Mock<IGetNextMobileIdQuery> getNextMobileIdQuery;

            public CeaseShould()
            {
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                loggerMock = new Mock<ILogger<MobilesService>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                getNextMobileIdQuery = new Mock<IGetNextMobileIdQuery>();

                expectedMobileGlobalId = Guid.NewGuid();
                expectedNewOrderGlobalId = Guid.NewGuid();
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    GlobalId = expectedMobileGlobalId,
                    State = Mobile.MobileState.Live.ToString()
                });
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedNewOrderGlobalId);

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobileGlobalId))
                    .Returns(expectedMobile);

                sut = new MobilesService(loggerMock.Object, mobileRepositoryMock.Object, guidCreatorMock.Object, getNextMobileIdQuery.Object);
            }

            [Fact]
            public void ReturnUpdatedMobile()
            {
                var actual = sut.Cease(expectedMobileGlobalId);

                actual.GlobalId.Should().Be(expectedMobileGlobalId);
                actual.State.Should().Be(Mobile.MobileState.ProcessingCease);
                actual.InProgressOrder.GlobalId.Should().Be(expectedNewOrderGlobalId);
                actual.InProgressOrder.CurrentState.Should().Be(Order.State.New);
                actual.InProgressOrder.Type.Should().Be(Order.OrderType.Cease);
            }

            [Fact]
            public void UpdateMobileInRepository()
            {
                sut.Cease(expectedMobileGlobalId);

                mobileRepositoryMock.Verify(x => x.Update(It.Is<Mobile>(y =>
                    y.GlobalId == expectedMobileGlobalId &&
                    y.State == Mobile.MobileState.ProcessingCease &&
                    y.InProgressOrder != null &&
                    y.InProgressOrder.CurrentState == Order.State.New &&
                    y.InProgressOrder.Type == Order.OrderType.Cease)));
            }
        }

        public class GetAvailablePhoneNumbersShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;
            private readonly Mobile expectedMobile;
            private readonly Guid expectedMobileGlobalId;
            private readonly Guid expectedNewOrderGlobalId;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<ILogger<MobilesService>> loggerMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;

            private readonly MobilesService sut;
            private readonly Mock<IGetNextMobileIdQuery> getNextMobileIdQuery;

            public GetAvailablePhoneNumbersShould()
            {
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                loggerMock = new Mock<ILogger<MobilesService>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                getNextMobileIdQuery = new Mock<IGetNextMobileIdQuery>();

                expectedMobileGlobalId = Guid.NewGuid();
                expectedNewOrderGlobalId = Guid.NewGuid();
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    GlobalId = expectedMobileGlobalId,
                    State = Mobile.MobileState.Live.ToString()
                });
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedNewOrderGlobalId);

                mobileRepositoryMock.Setup(x => x.GetById(expectedMobileGlobalId))
                    .Returns(expectedMobile);

                sut = new MobilesService(loggerMock.Object, mobileRepositoryMock.Object, guidCreatorMock.Object, getNextMobileIdQuery.Object);
            }

            [Fact]
            public void ReturnAvailablePhoneNumbers()
            {
                var expectedNextId = 101;

                getNextMobileIdQuery.Setup(x => x.Get()).Returns(expectedNextId);

                var actual = sut.GetAvailablePhoneNumbers();

                actual.First().Should().Be($"07{expectedNextId}000{expectedNextId}");
            }
        }
    }
}