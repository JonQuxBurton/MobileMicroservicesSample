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
    namespace ActivatReqeuestedHandlerSpec
    {
        public class HandleShould
        {
            private readonly ActivateRequestedHandler sut;
            private readonly Mock<IDataStore> dataStoreMock;
            private readonly Mock<ITransaction> transactionMock;
            private readonly Mock<IExternalMobileTelecomsNetworkService> externalMobileTelecomsNetworkServiceMock;
            private readonly ActivateRequestedMessage inputMessage;
            private readonly ExternalMobileTelecomsNetworkOrder expectedExternalServiceOrder;
            private readonly Mock<IMessagePublisher> messagePublisherMock;

            public HandleShould()
            {
                inputMessage = new ActivateRequestedMessage
                {
                    MobileOrderId = Guid.NewGuid(),
                    ActivationCode = "BAC132"
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
                var loggerMock = new Mock<ILogger<ActivateRequestedHandler>>();
                var monitoringMock = new Mock<IMonitoring>();

                sut = new ActivateRequestedHandler(loggerMock.Object, dataStoreMock.Object, externalMobileTelecomsNetworkServiceMock.Object, messagePublisherMock.Object, monitoringMock.Object);
            }

            [Fact]
            public async void AddActivateToDataStore()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostOrder(It.Is<ExternalMobileTelecomsNetworkOrder>(
                        y => y.Reference == expectedExternalServiceOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Add(
                    It.Is<Order>(y => y.MobileOrderId == expectedExternalServiceOrder.Reference &&
                                                y.Status == OrderStatus.New)));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void SendOrderToExternalService()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostOrder(It.Is<ExternalMobileTelecomsNetworkOrder>(
                        y => y.Reference == expectedExternalServiceOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                externalMobileTelecomsNetworkServiceMock.VerifyAll();
            }

            [Fact]
            public async void SetsOrderToSentInDataStore()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostOrder(It.Is<ExternalMobileTelecomsNetworkOrder>(
                        y => y.Reference == expectedExternalServiceOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Sent(expectedExternalServiceOrder.Reference));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void PublishActivationOrderSentMessage()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(
                        x => x.PostOrder(It.Is<ExternalMobileTelecomsNetworkOrder>(y => y.Reference == expectedExternalServiceOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                await sut.Handle(inputMessage);

                this.messagePublisherMock.Verify(x => x.PublishAsync(It.Is<ActivateOrderSentMessage>(y => y.MobileOrderId == inputMessage.MobileOrderId)));
            }

            [Fact]
            public async void ReturnTrueWhenSuccessful()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalMobileTelecomsNetworkOrder>()))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void RollbackWhenSendOrderFails()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalMobileTelecomsNetworkOrder>()))
                    .Returns(Task.FromResult(false));

                await sut.Handle(inputMessage);

                transactionMock.Verify(x => x.Rollback());
            }

            [Fact]
            public async void ReturnFalseWhenSendOrderFails()
            {
                externalMobileTelecomsNetworkServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalMobileTelecomsNetworkOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
