using DapperDataAccess;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MinimalEventBus.JustSaying;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using MobileTelecomsNetwork.EventHandlers.Handlers;
using MobileTelecomsNetwork.EventHandlers.Messages;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MobileTelecomsNetwork.EventHandlers.Tests.Handlers
{
    namespace CeaseRequestedHandlerSpec
    {
        public class HandleShould
        {
            private readonly CeaseRequestedHandler sut;
            private readonly Mock<IDataStore> dataStoreMock;
            private readonly Mock<ITransaction> transactionMock;
            private readonly Mock<IExternalMobileTelecomsNetworkService> externalMobileTelecomsNetworkServiceMock;
            private readonly CeaseRequestedMessage inputMessage;
            private readonly ExternalMobileTelecomsNetworkOrder expectedExternalServiceOrder;
            private readonly Mock<IMessagePublisher> messagePublisherMock;

            public HandleShould()
            {
                inputMessage = new CeaseRequestedMessage
                {
                    MobileOrderId = Guid.NewGuid()
                };
                expectedExternalServiceOrder = new ExternalMobileTelecomsNetworkOrder
                {
                    Reference = inputMessage.MobileOrderId
                };

                transactionMock = new Mock<ITransaction>();
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);

                externalMobileTelecomsNetworkServiceMock = new Mock<IExternalMobileTelecomsNetworkService>();
                messagePublisherMock = new Mock<IMessagePublisher>();
                var loggerMock = new Mock<ILogger<CeaseRequestedHandler>>();
                var monitoringMock = new Mock<IMonitoring>();

                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostCease(It.Is<ExternalMobileTelecomsNetworkOrder>(
                    y => y.Reference == expectedExternalServiceOrder.Reference)))
                        .Returns(Task.FromResult(true));

                sut = new CeaseRequestedHandler(loggerMock.Object, dataStoreMock.Object, externalMobileTelecomsNetworkServiceMock.Object, messagePublisherMock.Object, monitoringMock.Object);
            }

            [Fact]
            public async void AddCeaseToDataStore()
            {
                var actual = await sut.Handle(inputMessage);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Add(
                    It.Is<Order>(y => y.MobileOrderId == expectedExternalServiceOrder.Reference &&
                                                y.Type == OrderType.Cease &&
                                                y.Status == OrderStatus.New)));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void SendOrderToExternalService()
            {
                await sut.Handle(inputMessage);

                externalMobileTelecomsNetworkServiceMock.VerifyAll();
            }

            [Fact]
            public async void SetsOrderToSentInDataStore()
            {
                var actual = await sut.Handle(inputMessage);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Sent(expectedExternalServiceOrder.Reference));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void PublishOrderSentMessage()
            {
                await sut.Handle(inputMessage);

                this.messagePublisherMock.Verify(x => x.PublishAsync(It.Is<CeaseOrderSentMessage>(y => y.MobileOrderId == inputMessage.MobileOrderId)));
            }

            [Fact]
            public async void ReturnTrueWhenSuccessful()
            {
                var actual = await sut.Handle(inputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void RollbackWhenSendOrderFails()
            {
                externalMobileTelecomsNetworkServiceMock.Reset();
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostCease(It.IsAny<ExternalMobileTelecomsNetworkOrder>()))
                    .Returns(Task.FromResult(false));

                await sut.Handle(inputMessage);

                transactionMock.Verify(x => x.Rollback());
            }

            [Fact]
            public async void ReturnFalseWhenSendOrderFails()
            {
                externalMobileTelecomsNetworkServiceMock.Reset();
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostCease(It.IsAny<ExternalMobileTelecomsNetworkOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
