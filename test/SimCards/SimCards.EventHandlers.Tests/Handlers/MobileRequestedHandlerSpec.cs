using DapperDataAccess;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SimCards.EventHandlers.Data;
using SimCards.EventHandlers.Services;
using SimCards.EventHandlers.Handlers;
using SimCards.EventHandlers.Messages;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SimCards.EventHandlers.Tests.Handlers
{
    public class MobileRequestHandlerSpec
    {
        public class HandleShould
        {
            private MobileRequestedHandler sut;
            private Mock<ISimCardOrdersDataStore> dataStoreMock;
            private Mock<ITransaction> transactionMock;
            private Mock<ISimCardWholesaleService> simCardWholesaleServiceMock;
            private MobileRequestedMessage inputMessage;
            private SimCardOrder existingSimCardOrder;

            public HandleShould()
            {
                inputMessage = new MobileRequestedMessage
                {
                    Name = "Neil Armstrong",
                    MobileOrderId = Guid.NewGuid()
                };

                existingSimCardOrder = new SimCardOrder
                {
                    Name = "Alan Turing",
                    MobileOrderId = Guid.NewGuid()
                };

                transactionMock = new Mock<ITransaction>();
                dataStoreMock = new Mock<ISimCardOrdersDataStore>();
                dataStoreMock.Setup(x => x.BeginTransaction()).Returns(transactionMock.Object);
                dataStoreMock.Setup(x => x.GetExisting(existingSimCardOrder.MobileOrderId)).Returns(existingSimCardOrder);

                simCardWholesaleServiceMock = new Mock<ISimCardWholesaleService>();
                var loggerMock = new Mock<ILogger<MobileRequestedHandler>>();

                sut = new MobileRequestedHandler(loggerMock.Object, dataStoreMock.Object, simCardWholesaleServiceMock.Object);
            }

            [Fact]
            public async void ReturnTrueWhenSuccessful()
            {
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.IsAny<SimCardWholesalerOrder>()))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void SendOrderToWholesaler()
            {
                var expectedSimCardWholesalerOrder = new SimCardWholesalerOrder
                {
                    Name = inputMessage.Name,
                    Reference = inputMessage.MobileOrderId
                };
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.Is<SimCardWholesalerOrder>(
                        y => y.Name == expectedSimCardWholesalerOrder.Name && y.Reference == expectedSimCardWholesalerOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                simCardWholesaleServiceMock.VerifyAll();
            }

            [Fact]
            public async void SaveOrder()
            {
                var expectedSimCardWholesalerOrder = new SimCardWholesalerOrder
                {
                    Name = inputMessage.Name,
                    Reference = inputMessage.MobileOrderId
                };
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.Is<SimCardWholesalerOrder>(
                        y => y.Name == expectedSimCardWholesalerOrder.Name && y.Reference == expectedSimCardWholesalerOrder.Reference
                    )))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(inputMessage);

                dataStoreMock.Verify(x => x.Add(
                    It.Is<SimCardOrder>(y => y.Name == expectedSimCardWholesalerOrder.Name && y.MobileOrderId == expectedSimCardWholesalerOrder.Reference)));
                transactionMock.Verify(x => x.Dispose());
            }

            [Fact]
            public async void ReturnTrueWhenOrderAlreadyExists()
            {
                var existingInputMessage = new MobileRequestedMessage
                {
                    Name = existingSimCardOrder.Name,
                    MobileOrderId = existingSimCardOrder.MobileOrderId
                };

                var simCardWholesaleServiceMock = new Mock<ISimCardWholesaleService>();
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.IsAny<SimCardWholesalerOrder>()))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(existingInputMessage);

                actual.Should().BeTrue();
            }

            [Fact]
            public async void DoesNotPostOrderWhenOrderAlreadyExists()
            {
                var existingInputMessage = new MobileRequestedMessage
                {
                    Name = existingSimCardOrder.Name,
                    MobileOrderId = existingSimCardOrder.MobileOrderId
                };

                var simCardWholesaleServiceMock = new Mock<ISimCardWholesaleService>();
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.IsAny<SimCardWholesalerOrder>()))
                    .Returns(Task.FromResult(true));

                var actual = await sut.Handle(existingInputMessage);

                simCardWholesaleServiceMock.Verify(x => x.PostOrder(It.IsAny<SimCardWholesalerOrder>()), Times.Never);
            }

            [Fact]
            public async void RollbackWhenSendOrderFails()
            {
                var expectedSimCardWholesalerOrder = new SimCardWholesalerOrder
                {
                    Name = inputMessage.Name,
                    Reference = inputMessage.MobileOrderId
                };
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.IsAny<SimCardWholesalerOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(inputMessage);

                transactionMock.Verify(x => x.Rollback());
            }

            [Fact]
            public async void ReturnFalseWhenSendOrderFails()
            {
                var expectedSimCardWholesalerOrder = new SimCardWholesalerOrder
                {
                    Name = inputMessage.Name,
                    Reference = inputMessage.MobileOrderId
                };
                simCardWholesaleServiceMock.Setup(x => x.PostOrder(It.IsAny<SimCardWholesalerOrder>()))
                    .Returns(Task.FromResult(false));

                var actual = await sut.Handle(inputMessage);

                actual.Should().BeFalse();
            }
        }
    }
}
