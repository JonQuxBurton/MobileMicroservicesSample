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

namespace Mobiles.Api.Tests.Handlers
{
    namespace ProvisioningOrderSentHandlerSpec
    {
        public class HandleShould
        {
            private readonly OrderCompletedHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly ProvisionOrderCompletedMessage inputMessage;

            public HandleShould()
            {
                var inFlightOrder = new Order(new OrderDataEntity()
                {
                    State = "Sent"
                });
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = "ProcessingProvisioning"
                }, inFlightOrder);
                inputMessage = new ProvisionOrderCompletedMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<OrderCompletedHandler>>();
                var monitoringMock = new Mock<IMonitoring>();

                getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                var serviceProviderMock = ServiceProviderHelper.GetMock();
                serviceProviderMock.Setup(x => x.GetService(typeof(IGetMobileByOrderIdQuery))).Returns(getMobileByOrderIdQueryMock.Object);
                serviceProviderMock.Setup(x => x.GetService(typeof(IRepository<Mobile>))).Returns(mobileRepositoryMock.Object);

                sut = new OrderCompletedHandler(loggerMock.Object, monitoringMock.Object, serviceProviderMock.Object);
            }

            [Fact]
            public async void CompleteTheMobilesProvisioning()
            {
                await sut.Handle(inputMessage);

                expectedMobile.CurrentState.Should().Be(Mobile.State.WaitingForActivation);
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
                var nonExistantMobileMessage = new ProvisionOrderCompletedMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
