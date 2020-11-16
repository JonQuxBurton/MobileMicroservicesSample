using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Handlers;
using Mobiles.Api.Messages;
using Moq;
using System;
using System.Collections.Generic;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Xunit;

namespace Mobiles.Api.Tests.Handlers
{
    namespace CeaseOrderCompletedHandlerSpec
    {
        public class HandleShould
        {
            private readonly CeaseOrderCompletedHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly CeaseOrderCompletedMessage inputMessage;

            public HandleShould()
            {
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                var inFlightOrder = new Order(new OrderDataEntity()
                {
                    State = "Sent"
                });
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = "ProcessingCease",
                    Orders = new List<OrderDataEntity>() { inFlightOrder.GetDataEntity() }
                });
                inputMessage = new CeaseOrderCompletedMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<CeaseOrderCompletedHandler>>();
                var monitoringMock = new Mock<IMonitoring>();

                getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                var serviceProviderMock = ServiceProviderHelper.GetMock();
                serviceProviderMock.Setup(x => x.GetService(typeof(IGetMobileByOrderIdQuery))).Returns(getMobileByOrderIdQueryMock.Object);
                serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Mobile>))).Returns(mobileRepositoryMock.Object);

                sut = new CeaseOrderCompletedHandler(loggerMock.Object, monitoringMock.Object, serviceProviderMock.Object);
            }

            [Fact]
            public async void SetTheMobilesToCeased()
            {
                await sut.Handle(inputMessage);

                expectedMobile.State.Should().Be(Mobile.MobileState.Ceased);
            }

            [Fact]
            public async void HaveCompleteTheInFlightOrder()
            {
                await sut.Handle(inputMessage);

                expectedMobile.InFlightOrder.Should().BeNull();
            }

            [Fact]
            public async void UpdateTheMobileInTheRepository()
            {
                await sut.Handle(inputMessage);

                this.mobileRepositoryMock.Verify(x => x.Update(expectedMobile));
            }

            [Fact]
            public async void ReturnTrueWhenSuccessful()
            {
                var actual = await sut.Handle(inputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void ReturnFalseWhenNotSuccessful()
            {
                var nonExistantMobileMessage = new CeaseOrderCompletedMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
