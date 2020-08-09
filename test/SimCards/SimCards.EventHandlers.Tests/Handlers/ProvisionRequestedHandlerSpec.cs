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
    public static class MobileRequestHandlerSpec
    {
        public class HandleShould
        {
            private readonly ProvisionRequestedHandler sut;
            private readonly Mock<ISimCardOrdersDataStore> dataStoreMock;
            private readonly Mock<ITransaction> transactionMock;
            private readonly Mock<IExternalSimCardsProviderService> externalSimCardProviderServiceMock;
            private readonly ProvisionRequestedMessage inputMessage;
            private readonly SimCardOrder existingSimCardOrder;

            public HandleShould()
            {
                inputMessage = new ProvisionRequestedMessage
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

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void SendOrderToExternalProvider()
            {
                var expectedExternalSimCardOrder = new ExternalSimCardOrder
                {
                    Name = inputMessage.Name,
                    MobileReference = inputMessage.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.Is<ExternalSimCardOrder>(
                        y => y.Name == expectedExternalSimCardOrder.Name && y.MobileReference == expectedExternalSimCardOrder.MobileReference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                externalSimCardProviderServiceMock.VerifyAll();
            }

            [Fact]
            public async void SaveOrder()
            {
                var expectedExternalSimCardOrder = new ExternalSimCardOrder
                {
                    Name = inputMessage.Name,
                    MobileReference = inputMessage.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.Is<ExternalSimCardOrder>(
                        y => y.Name == expectedExternalSimCardOrder.Name && y.MobileReference == expectedExternalSimCardOrder.MobileReference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                dataStoreMock.Verify(x => x.Add(
                    It.Is<SimCardOrder>(y => y.Name == expectedExternalSimCardOrder.Name && 
                                                        y.MobileId == expectedExternalSimCardOrder.MobileReference)));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void ReturnTrueWhenOrderAlreadyExists()
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
                    Name = inputMessage.Name,
                    MobileReference = inputMessage.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(inputMessage);

                transactionMock.Verify(x => x.Rollback());
            }

            [Fact]
            public async void ReturnFalseWhenSendOrderFails()
            {
                var expectedExternalSimCardOrder = new ExternalSimCardOrder
                {
                    Name = inputMessage.Name,
                    MobileReference = inputMessage.MobileId,
                };
                externalSimCardProviderServiceMock.Setup(x => x.PostOrder(It.IsAny<ExternalSimCardOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
