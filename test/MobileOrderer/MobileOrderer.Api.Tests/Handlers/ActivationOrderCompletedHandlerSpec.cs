using FluentAssertions;
using Microsoft.Extensions.Logging;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Handlers;
using MobileOrderer.Api.Messages;
using Moq;
using System;
using Utils.DomainDrivenDesign;
using Xunit;
using static MobileOrderer.Api.Domain.Mobile;

namespace MobileOrderer.Api.Tests.Handlers
{
    public class ActivationOrderCompletedHandlerSpec
    {
        public class HandleShould
        {
            private readonly ActivationOrderCompletedHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly ActivationOrderCompletedMessage inputMessage;

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
                inputMessage = new ActivationOrderCompletedMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<ActivationOrderCompletedHandler>>();

                this.getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                sut = new ActivationOrderCompletedHandler(loggerMock.Object, mobileRepositoryMock.Object, getMobileByOrderIdQueryMock.Object);
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
                var nonExistantMobileMessage = new ActivationOrderCompletedMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
