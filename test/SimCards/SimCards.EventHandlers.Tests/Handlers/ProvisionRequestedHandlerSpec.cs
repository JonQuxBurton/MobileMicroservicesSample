using DapperDataAccess;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Handlers;
using SimCards.EventHandlers.Messages;
using System;
using System.Threading.Tasks;
using Xunit;
using MinimalEventBus.JustSaying;
using SimCards.EventHandlers.Domain;

namespace SimCards.EventHandlers.Tests.Handlers
{
    namespace MobileRequestHandlerSpec
    {
        public class HandleShould
        {
            private readonly ProvisionRequestedHandler sut;
            private readonly Mock<ISimCardOrdersDataStore> dataStoreMock;
            private readonly Mock<ITransaction> transactionMock;
            private readonly Mock<IExternalSimCardsProviderService> externalSimCardProviderServiceMock;
            private readonly ProvisionRequestedMessage receivedEvent;
            private readonly SimCardOrder existingSimCardOrder;

            public HandleShould()
            {
                receivedEvent = new ProvisionRequestedMessage
                {
                    Name = "Neil Armstrong",
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid()
                };

                existingSimCardOrder = new SimCardOrder
                {
                    Name = "Alan Turing",
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid()
                };

                transactionMock = new Mock<ITransaction>();
                dataStoreMock = new Mock<ISimCardOrdersDataStore>();
                dataStoreMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
                dataStoreMock.Setup(x => x.GetExisting(existingSimCardOrder.MobileId, existingSimCardOrder.MobileOrderId)).Returns(existingSimCardOrder);

                externalSimCardProviderServiceMock = new Mock<IExternalSimCardsProviderService>();
                var loggerMock = new Mock<ILogger<ProvisionRequestedHandler>>();
                var messagePublisherMock = new Mock<IMessagePublisher>();
                var monitoringMock = new Mock<IMonitoring>();

                sut = new ProvisionRequestedHandler(loggerMock.Object, dataStoreMock.Object, externalSimCardProviderServiceMock.Object, messagePublisherMock.Object, monitoringMock.Object);
            }

            [Fact]
            public async void ReturnTrueWhenSuccessful()
            {
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(receivedEvent);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void SendOrderToExternalProvider()
            {
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.Is<ExternalSimCardOrder>(
                        y => y.PhoneNumber == receivedEvent.PhoneNumber &&
                            y.Name == receivedEvent.Name && 
                            y.Reference == receivedEvent.MobileOrderId
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(receivedEvent);

                externalSimCardProviderServiceMock.VerifyAll();
            }

            [Fact]
            public async void SaveOrder()
            {
                var expectedExternalSimCardOrder = new ExternalSimCardOrder
                {
                    Name = receivedEvent.Name,
                    Reference = receivedEvent.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.Is<ExternalSimCardOrder>(
                        y => y.Name == expectedExternalSimCardOrder.Name && y.Reference == expectedExternalSimCardOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(receivedEvent);

                dataStoreMock.Verify(x => x.Add(
                    It.Is<SimCardOrder>(y => y.Name == expectedExternalSimCardOrder.Name && 
                                                        y.MobileId == expectedExternalSimCardOrder.Reference)));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void ReturnTrueWhenOrderAlreadyExists()
            {
                var existingInputMessage = new ProvisionRequestedMessage
                {
                    Name = existingSimCardOrder.Name,
                    MobileId = existingSimCardOrder.MobileId,
                    MobileOrderId = existingSimCardOrder.MobileOrderId
                };

                var actual = await sut.Handle(existingInputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void DoesNotPostOrderWhenOrderAlreadyExists()
            {
                var existingInputMessage = new ProvisionRequestedMessage
                {
                    Name = existingSimCardOrder.Name,
                    MobileOrderId = existingSimCardOrder.MobileOrderId
                };

                var externalSimCardProviderServiceMock = new Mock<IExternalSimCardsProviderService>();
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(existingInputMessage);

                externalSimCardProviderServiceMock.Verify(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()), Times.Never);
            }

            [Fact]
            public async void RollbackWhenSendOrderFails()
            {
                var expectedExternalSimCardOrder = new ExternalSimCardOrder
                {
                    Name = receivedEvent.Name,
                    Reference = receivedEvent.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(receivedEvent);

                transactionMock.Verify(x => x.Rollback());
            }

            [Fact]
            public async void ReturnFalseWhenSendOrderFails()
            {
                var expectedExternalSimCardOrder = new ExternalSimCardOrder
                {
                    Name = receivedEvent.Name,
                    Reference = receivedEvent.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(receivedEvent);

                actual.Should().BeFalse();
            }
        }
    }
}
