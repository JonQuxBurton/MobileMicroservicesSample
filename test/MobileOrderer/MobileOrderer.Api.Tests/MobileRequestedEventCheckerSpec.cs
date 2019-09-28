using MinimalEventBus.JustSaying;
using MobileOrderer.Api.Data;
using MobileOrderer.Api.Services;
using Moq;
using System;
using Xunit;

namespace MobileOrderer.Api.Tests
{
    public class MobileRequestedEventCheckerSpec
    {
        private readonly MobileOrder expectedNewOrder;
        private readonly MobileRequestedEventChecker sut;
        private readonly Mock<IOrdersDataStore> ordersDataStoreMock;

        public MobileRequestedEventCheckerSpec()
        {
            expectedNewOrder = new MobileOrder
            {
                GlobalId = Guid.NewGuid(),
                Name = "Neil Armstrong",
                ContactPhoneNumber = "0123456789",
                CreatedAt = new DateTime(2001, 5, 4, 9, 10, 11),
                Status = "New"
            };
            ordersDataStoreMock = new Mock<IOrdersDataStore>();
            ordersDataStoreMock.Setup(x => x.GetNewOrders()).Returns(new[] { expectedNewOrder });
            var messagePublisher = new Mock<IMessagePublisher>();
            sut = new MobileRequestedEventChecker(ordersDataStoreMock.Object, messagePublisher.Object);
        }

        [Fact]
        public void CallSetToProcessingWithNewOrder()
        {
            sut.Check();

            ordersDataStoreMock.Verify(x => x.SetToProcessing(expectedNewOrder));
        }

        [Fact]
        public void SetsOrderToProcessing()
        {
            sut.Check();

            ordersDataStoreMock.Verify(x => x.SetToProcessing(expectedNewOrder));
        }
    }
}
