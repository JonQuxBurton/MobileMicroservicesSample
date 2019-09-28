using DapperDataAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimCardWholesaler.Api.Controllers;
using SimCardWholesaler.Api.Data;
using SimCardWholesaler.Api.Resources;
using System;
using Xunit;

namespace SimCardWholesaler.Api.Tests
{
    public class OrdersControllerSpec
    {
        public class StatusShould
        {
            public StatusShould()
            {
                ordersDataStoreMock = new Mock<IOrdersDataStore>();

                sut = new OrdersController(ordersDataStoreMock.Object);
            }

            private readonly OrdersController sut;
            private readonly Mock<IOrdersDataStore> ordersDataStoreMock;

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Status();

                actual.Should().BeOfType<OkResult>();
            }
        }

        public class GetShould
        {
            private readonly Order expectedOrder;
            private readonly OrdersController sut;

            public GetShould()
            {
                expectedOrder = new Order
                {
                    Id = 101,
                    Reference = Guid.NewGuid(),
                    Status = "Processing"
                };
                var ordersDataStoreMock = new Mock<IOrdersDataStore>();
                ordersDataStoreMock.Setup(x => x.GetByReference(expectedOrder.Reference))
                    .Returns(expectedOrder);
                sut = new OrdersController(ordersDataStoreMock.Object);
            }

            [Fact]
            public void ReturnOkObjectResult()
            {
                var actual = sut.Get(expectedOrder.Reference);

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnOrder()
            {
                var actual = sut.Get(expectedOrder.Reference);

                var actualOrder = actual.As<OkObjectResult>().Value;
                actualOrder.Should().Be(expectedOrder);
            }

            [Fact]
            public void ReturnNotFoundWhenOrderNotFound()
            {
                var actual = sut.Get(Guid.Empty);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }

        public class CreateShould
        {
            private readonly OrdersController sut;
            private readonly Mock<IOrdersDataStore> ordersDataStoreMock;
            private readonly Mock<ITransaction> transactionMock;

            public CreateShould()
            {
                transactionMock = new Mock<ITransaction>();
                ordersDataStoreMock = new Mock<IOrdersDataStore>();
                ordersDataStoreMock.Setup(x => x.BeginTransaction())
                    .Returns(transactionMock.Object);

                sut = new OrdersController(ordersDataStoreMock.Object);
            }

            [Fact]
            public void ReturnOk()
            {
                var expectedOrder = new OrderToAdd
                {
                    Reference = Guid.NewGuid()
                };

                var actual = sut.Create(expectedOrder);

                actual.Should().BeOfType<OkResult>();
            }

            [Fact]
            public void AddOrder()
            {
                var expectedOrder = new OrderToAdd
                {
                    Reference = Guid.NewGuid()
                };

                sut.Create(expectedOrder);

                ordersDataStoreMock.Verify(x => x.Add(It.Is<Order>(y => y.Reference == expectedOrder.Reference && y.Status == "New")));
                transactionMock.Verify(x => x.Dispose());
            }
        }
    }
}
