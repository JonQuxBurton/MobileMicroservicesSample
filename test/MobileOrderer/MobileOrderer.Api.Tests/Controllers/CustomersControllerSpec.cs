using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MobileOrderer.Api.Controllers;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
using Moq;
using System;
using System.Collections.Immutable;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;

namespace MobileOrderer.Api.Tests.Controllers
{
    public static class CustomersControllerSpec
    {
        public class GetShould
        {
            public GetShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                var loggingMock = new Mock<ILogger<CustomersController>>();

                sut = new CustomersController(
                    loggingMock.Object,
                    customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    guidCreatorMock.Object);

                expected = new Customer();

                customerRepositoryMock.Setup(x => x.GetById(expected.GlobalId))
                    .Returns(expected);
            }

            private readonly CustomersController sut;
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Customer expected;

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Get(expected.GlobalId);

                actual.Result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnCustomer()
            {
                var actual = sut.Get(expected.GlobalId);

                var actualResult = actual.Result as OkObjectResult;
                actualResult.Value.Should().Be(expected);
            }

            [Fact]
            public void ReturnNotFound()
            {
                var actual = sut.Get(Guid.NewGuid());

                actual.Result.Should().BeOfType<NotFoundResult>();
            }
        }

        public class GetAllShould
        {
            public GetAllShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                var loggingMock = new Mock<ILogger<CustomersController>>();

                sut = new CustomersController(loggingMock.Object,
                    customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    guidCreatorMock.Object);

                expected = ImmutableList.Create(
                    new Customer { GlobalId = Guid.NewGuid() },
                    new Customer { GlobalId = Guid.NewGuid() }
                    );

                customerRepositoryMock.Setup(x => x.GetAll())
                    .Returns(expected);
            }

            private readonly CustomersController sut;
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly ImmutableList<Customer> expected;

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.GetAll();

                actual.Result.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnAllCustomers()
            {
                var actualActionResult = sut.GetAll();

                var actualResult = actualActionResult.Result as OkObjectResult;
                var actual = actualResult.Value as Customer[];
                
                actual.Should().Equal(expected);
            }
        }

        public class CreateShould
        {
            public CreateShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();

                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                var loggingMock = new Mock<ILogger<CustomersController>>();

                customerToAdd = new CustomerToAdd()
                {
                    Name = "Armstrong Corporation"
                };

                expectedCustomer = new Customer
                {
                    Name = customerToAdd.Name,
                    GlobalId = Guid.NewGuid()
                };

                guidCreatorMock.Setup(x => x.Create())
                    .Returns(expectedCustomer.GlobalId);

                customerRepositoryMock.Setup(x => x.GetById(expectedCustomer.GlobalId))
                    .Returns(expectedCustomer);

                sut = new CustomersController(
                    loggingMock.Object,
                    customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    guidCreatorMock.Object);
            }

            private readonly CustomersController sut;
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly CustomerToAdd customerToAdd;
            private readonly Customer expectedCustomer;

            [Fact]
            public void AddCustomerToRepository()
            {
                sut.Create(customerToAdd);

                customerRepositoryMock.Verify(x => x.Add(It.Is<Customer>(y =>
                    y.GlobalId == expectedCustomer.GlobalId &&
                    y.Name == customerToAdd.Name)));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Create(customerToAdd);

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNewCustomer()
            {
                var actual = sut.Create(customerToAdd);
                var actualResult = actual as OkObjectResult;
                var actualCustomer = actualResult.Value as CustomerResource;

                actualCustomer.Should().NotBeNull();
                actualCustomer.GlobalId.Should().Be(expectedCustomer.GlobalId);
                actualCustomer.Name.Should().Be(expectedCustomer.Name);
            }
        }

        public class ProvisionShould
        {
            public ProvisionShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();

                expectedGlobalId = Guid.NewGuid();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedGlobalId);
                var loggingMock = new Mock<ILogger<CustomersController>>();

                expectedCustomer = new Customer
                { 
                    Name = "Armstrong Corporation",
                    GlobalId = Guid.NewGuid()
                };

                customerRepositoryMock.Setup(x => x.GetById(expectedCustomer.GlobalId))
                    .Returns(expectedCustomer);

                sut = new CustomersController(
                    loggingMock.Object,
                    customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    guidCreatorMock.Object);
            }

            private readonly CustomersController sut;
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Customer expectedCustomer;
            private readonly Guid expectedGlobalId;

            [Fact]
            public void AddMobileToRepositoryWithStateOfNew()
            {
                var expectedOrder = new OrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };

                sut.Provision(expectedCustomer.GlobalId, expectedOrder);

                mobileRepositoryMock.Verify(x => x.Add(It.Is<Mobile>(y =>
                    y.GlobalId == expectedGlobalId &&
                    y.CurrentState == Mobile.State.New &&
                    y.CustomerId == expectedCustomer.GlobalId)));
            }

            [Fact]
            public void AddMobileToRepositoryWithInFligthOrder()
            {
                var expectedOrder = new OrderToAdd()
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };

                sut.Provision(expectedCustomer.GlobalId, expectedOrder);

                mobileRepositoryMock.Verify(x => x.Add(It.Is<Mobile>(y =>
                    y.GlobalId == expectedGlobalId &&
                    y.CurrentState == Mobile.State.New &&
                    y.InFlightOrder != null &&
                    y.InFlightOrder.CurrentState == Order.State.New)));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Provision(expectedCustomer.GlobalId, new OrderToAdd());

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNewMobile()
            {
                var actual = sut.Provision(expectedCustomer.GlobalId, new OrderToAdd());
                var actualResult = actual as OkObjectResult;
                var actualMobile = actualResult.Value as MobileResource;

                actualMobile.Should().NotBeNull();
                actualMobile.GlobalId.Should().Be(expectedGlobalId);
                actualMobile.CustomerId.Should().Be(expectedCustomer.GlobalId);
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();

                guidCreatorMock.Setup(x => x.Create()).Returns(notFoundGlobalId);

                var actual = sut.Provision(notFoundGlobalId, new OrderToAdd());

                actual.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}
