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

namespace MobileOrderer.Api.Tests.Handlers
{
    public class CeaseOrderSentHandlerSpec
    {
        public class HandleShould
        {
            private readonly CeaseOrderSentHandler sut;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGetMobileByOrderIdQuery> getMobileByOrderIdQueryMock;

            private readonly Mobile expectedMobile;
            private readonly CeaseOrderSentMessage inputMessage;

            public HandleShould()
            {
                var inFlightOrder = new Order(new OrderDataEntity()
                {
                    State = "Processing"
                });
                expectedMobile = new Mobile(new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    State = "ProcessingCease"
                }, inFlightOrder);
                inputMessage = new CeaseOrderSentMessage()
                {
                    MobileOrderId = expectedMobile.GlobalId
                };

                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                getMobileByOrderIdQueryMock = new Mock<IGetMobileByOrderIdQuery>();
                var loggerMock = new Mock<ILogger<CeaseOrderSentHandler>>();

                this.getMobileByOrderIdQueryMock.Setup(x => x.Get(inputMessage.MobileOrderId))
                    .Returns(expectedMobile);

                sut = new CeaseOrderSentHandler(loggerMock.Object, mobileRepositoryMock.Object, getMobileByOrderIdQueryMock.Object);
            }

            [Fact]
            public async void SetTheMobilesInFlightOrderToSent()
            {
                await sut.Handle(inputMessage);

                expectedMobile.CurrentState.Should().Be(Mobile.State.ProcessingCease);
                expectedMobile.InFlightOrder.CurrentState.Should().Be(Order.State.Sent);
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
                var nonExistantMobileMessage = new CeaseOrderSentMessage()
                {
                    MobileOrderId = Guid.Empty
                };

                var actual = await sut.Handle(nonExistantMobileMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
