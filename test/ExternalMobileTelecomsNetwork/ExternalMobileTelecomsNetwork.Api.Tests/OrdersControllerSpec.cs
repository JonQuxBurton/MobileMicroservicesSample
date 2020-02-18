using ExternalMobileTelecomsNetwork.Api.Controllers;
using ExternalMobileTelecomsNetwork.Api.Data;
using ExternalMobileTelecomsNetwork.Api.Resources;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

namespace ExternalMobileTelecomsNetwork.Api.Tests
{
    public class OrdersControllerSpec
    {
        public class GetShould
        {
            private Order expectedOrder;
            private Mock<IDataStore> dataStoreMock;
            private OrdersController sut;

            public GetShould()
            {
                expectedOrder = new Order
                {
                    Reference = Guid.NewGuid()
                };
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetByReference(expectedOrder.Reference))
                    .Returns(expectedOrder);
                sut = new OrdersController(dataStoreMock.Object);
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Get(expectedOrder.Reference);

                actual.Result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnExpectedOrder()
            {
                var actual = sut.Get(expectedOrder.Reference);

                var actualResult = actual.Result as OkObjectResult;
                actualResult.Value.Should().Be(expectedOrder);
            }

            [Fact]
            public void ReturnNotFound()
            {
                var nonExistantId = Guid.NewGuid();
                
                var actual = sut.Get(nonExistantId);

                actual.Result.Should().BeOfType<NotFoundResult>();
            }
        }

        public class CreateShould
        {
            private readonly OrdersController sut;
            private readonly Mock<IDataStore> dataStoreMock;
            private readonly OrderToAdd expectedOrderToAdd;

            public CreateShould()
            {
                expectedOrderToAdd = new OrderToAdd()
                {
                    Reference = Guid.NewGuid(),
                    Name = "Neil Armstrong"
                };
                dataStoreMock = new Mock<IDataStore>();
                
                sut = new OrdersController(dataStoreMock.Object);
            }

            [Fact]
            public void CreateTheOrder()
            {
                sut.Create(expectedOrderToAdd);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Add(It.Is<Order>(
                    y => y.Name == expectedOrderToAdd.Name &&
                    y.Reference == expectedOrderToAdd.Reference
                )));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Create(expectedOrderToAdd);

                actual.Should().BeOfType<OkResult>();
            }
        }

        public class CompleteShould
        {
            private readonly OrdersController sut;
            private readonly Mock<IDataStore> dataStoreMock;
            private readonly OrderToComplete expectedOrderToComplete;
            private readonly Order expectedOrder;

            public CompleteShould()
            {
                expectedOrderToComplete = new OrderToComplete()
                {
                    Reference = Guid.NewGuid()
                };
                expectedOrder = new Order { };
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetByReference(expectedOrderToComplete.Reference))
                    .Returns(expectedOrder);

                sut = new OrdersController(dataStoreMock.Object);
            }

            [Fact]
            public void CompleteTheOrder()
            {
                sut.Complete(expectedOrderToComplete);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Complete(expectedOrder));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Complete(expectedOrderToComplete);

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundOrderToComplete = new OrderToComplete
                {
                    Reference = Guid.NewGuid()
                };
                dataStoreMock.Setup(x => x.GetByReference(notFoundOrderToComplete.Reference))
                    .Returns((Order)null);

                var actual = sut.Complete(notFoundOrderToComplete);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }

        public class StatusShould
        {
            [Fact]
            public void ReturnOk()
            {
                var dataStoreMock = new Mock<IDataStore>();
                var sut = new OrdersController(dataStoreMock.Object);
                var actual = sut.Status();

                actual.Should().BeOfType<OkResult>();
            }
        }
    }
}
