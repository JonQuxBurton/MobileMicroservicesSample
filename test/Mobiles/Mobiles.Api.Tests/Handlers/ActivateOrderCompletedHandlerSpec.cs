using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Handlers;
using Mobiles.Api.Messages;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Xunit;
using static Mobiles.Api.Domain.Mobile;

namespace Mobiles.Api.Tests.Handlers
{
    namespace ActivateOrderCompletedHandlerSpec
    {
        public class HandleShould
        {
            private readonly ActivateOrderCompletedHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly ActivateOrderCompletedMessage inputMessage;

            public HandleShould()
            {
                var inFlightOrder = new Order(new OrderDataEntity()
                {
                    State = "Sent"
                });
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = "ProcessingActivation"
                }, inFlightOrder);
                inputMessage = new ActivateOrderCompletedMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<ActivateOrderCompletedHandler>>();
                var monitoringMock = new Mock<IMonitoring>();

                getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                var serviceProviderMock = ServiceProviderHelper.GetMock();
                serviceProviderMock.Setup(x => x.GetService(typeof(IGetMobileByOrderIdQuery))).Returns(getMobileByOrderIdQueryMock.Object);
                serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Mobile>))).Returns(mobileRepositoryMock.Object);

                sut = new ActivateOrderCompletedHandler(loggerMock.Object, monitoringMock.Object, serviceProviderMock.Object);
            }

            [Fact]
            public async void ActivateTheMobile()
            {
                await sut.Handle(inputMessage);

                expectedMobile.CurrentState.Should().Be(State.Live);
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
                var nonExistantMobileMessage = new ActivateOrderCompletedMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
