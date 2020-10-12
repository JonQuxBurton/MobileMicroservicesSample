using DapperDataAccess;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ExternalSimCardsProvider.Api.Controllers;
using ExternalSimCardsProvider.Api.Data;
using ExternalSimCardsProvider.Api.Resources;
using System;
using Xunit;
using ExternalSimCardsProvider.Api.Domain;

namespace ExternalSimCardsProvider.Api.Tests
{
    namespace OrdersControllerSpec
    {
        public class GetShould
        {
            private readonly Order expectedOrder;
            private readonly OrdersController sut;
            private readonly Mock<IExternalMobileTelecomsNetworkService> externalMobileTelecomsNetworkService;

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
                var activationCodeGeneratorMock = new Mock<IActivationCodeGenerator>();
                externalMobileTelecomsNetworkService = new Mock<IExternalMobileTelecomsNetworkService>();

                sut = new OrdersController(ordersDataStoreMock.Object, 
                    activationCodeGeneratorMock.Object,
                    externalMobileTelecomsNetworkService.Object);
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
            private readonly Mock<IExternalMobileTelecomsNetworkService> externalMobileTelecomsNetworkService;

            public CreateShould()
            {
                transactionMock = new Mock<ITransaction>();
                ordersDataStoreMock = new Mock<IOrdersDataStore>();
                ordersDataStoreMock.Setup(x => x.BeginTransaction())
                    .Returns(transactionMock.Object);
                var activationCodeGeneratorMock = new Mock<IActivationCodeGenerator>();
                externalMobileTelecomsNetworkService = new Mock<IExternalMobileTelecomsNetworkService>();

                sut = new OrdersController(ordersDataStoreMock.Object, 
                    activationCodeGeneratorMock.Object,
                    externalMobileTelecomsNetworkService.Object);
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

        public class GetActivationCodeShould
        {
            private readonly Order expectedOrder;
            private readonly OrdersController sut;
            private readonly Mock<IExternalMobileTelecomsNetworkService> externalMobileTelecomsNetworkService;

            public GetActivationCodeShould()
            {
                expectedOrder = new Order
                {
                    Id = 101,
                    Reference = Guid.NewGuid(),
                    Status = "Processing",
                    ActivationCode = "BAC132"
                };
                var ordersDataStoreMock = new Mock<IOrdersDataStore>();
                ordersDataStoreMock.Setup(x => x.GetByReference(expectedOrder.Reference))
                    .Returns(expectedOrder);
                var activationCodeGeneratorMock = new Mock<IActivationCodeGenerator>();
                externalMobileTelecomsNetworkService = new Mock<IExternalMobileTelecomsNetworkService>();

                sut = new OrdersController(ordersDataStoreMock.Object, 
                    activationCodeGeneratorMock.Object,
                    externalMobileTelecomsNetworkService.Object);
            }

            [Fact]
            public void ReturnOkObjectResult()
            {
                var actual = sut.GetActivationCode(expectedOrder.Reference);

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnActivationCode()
            {
                var actualActionResult = sut.GetActivationCode(expectedOrder.Reference);

                var actual = actualActionResult.As<OkObjectResult>().Value as ActivationCodeResource;

                actual.ActivationCode.Should().Be(expectedOrder.ActivationCode);
            }

            [Fact]
            public void ReturnNoContentWhenNoActivationCode()
            {
                expectedOrder.ActivationCode = null;

                var actual = sut.GetActivationCode(expectedOrder.Reference);

                actual.Should().BeOfType<NoContentResult>();
            }

            [Fact]
            public void ReturnNotFoundWhenOrderNotFound()
            {
                var actual = sut.GetActivationCode(Guid.Empty);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}
