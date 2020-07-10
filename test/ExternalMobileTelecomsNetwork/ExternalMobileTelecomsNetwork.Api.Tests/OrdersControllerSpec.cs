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
                    Reference = Guid.NewGuid()
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
                    y => y.Reference == expectedOrderToAdd.Reference &&
                    y.Type == "Provision" &&
                    y.Status == "New"
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
            private readonly Order expectedOrder;
            private readonly Guid expectedReference;

            public CompleteShould()
            {
                expectedReference = Guid.NewGuid();
                expectedOrder = new Order { };
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetByReference(expectedReference))
                    .Returns(expectedOrder);

                sut = new OrdersController(dataStoreMock.Object);
            }

            [Fact]
            public void CompleteTheOrder()
            {
                sut.Complete(expectedReference);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Complete(expectedReference));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Complete(expectedReference);

                actual.Should().BeOfType<OkResult>();
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundReference = Guid.NewGuid();
                dataStoreMock.Setup(x => x.GetByReference(notFoundReference))
                    .Returns((Order)null);

                var actual = sut.Complete(notFoundReference);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }

        public class CancelShould
        {
            private readonly OrdersController sut;
            private readonly Mock<IDataStore> dataStoreMock;
            private readonly Order expectedOrder;
            private readonly Guid expectedReference;

            public CancelShould()
            {
                expectedReference = Guid.NewGuid();
                expectedOrder = new Order { };
                dataStoreMock = new Mock<IDataStore>();
                dataStoreMock.Setup(x => x.GetByReference(expectedReference))
                    .Returns(expectedOrder);

                sut = new OrdersController(dataStoreMock.Object);
            }

            [Fact]
            public void CreateTheCancelOrder()
            {
                sut.Cancel(expectedReference);

                dataStoreMock.Verify(x => x.BeginTransaction());
                dataStoreMock.Verify(x => x.Add(It.Is<Order>(
                    y => y.Reference == expectedReference && y.Type == "Cancel" && y.Status == "New"
                )));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Cancel(expectedReference);

                actual.Should().BeOfType<OkResult>();
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
