using System;
using System.Collections.Immutable;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Controllers;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using Moq;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;

namespace Mobiles.Api.Tests.Controllers
{
    namespace CustomersControllerSpec
    {
        public class GetShould
        {
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<ICustomersService> customersServiceMock;
            private readonly Customer expected;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMonitoring> monitoringMock;

            private readonly CustomersController sut;

            public GetShould()
            {
                customersServiceMock = new Mock<ICustomersService>();
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                var loggingMock = new Mock<ILogger<CustomersController>>();
                var getMobilesByCustomerIdQueryMock = new Mock<IGetMobilesByCustomerIdQuery>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                sut = new CustomersController(customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    getMobilesByCustomerIdQueryMock.Object,
                    this.customersServiceMock.Object);

                expected = new Customer();

                customerRepositoryMock.Setup(x => x.GetById(expected.GlobalId))
                    .Returns(expected);
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Get(expected.GlobalId);

                actual.Result.Should().BeOfType<OkObjectResult>();
            }
        }

        public class GetAllShould
        {
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly ImmutableList<Customer> expected;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Mock<ICustomersService> customersServiceMock;

            private readonly CustomersController sut;

            public GetAllShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                customersServiceMock = new Mock<ICustomersService>();
                var loggingMock = new Mock<ILogger<CustomersController>>();
                var getMobilesByCustomerIdQueryMock = new Mock<IGetMobilesByCustomerIdQuery>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                sut = new CustomersController(customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    getMobilesByCustomerIdQueryMock.Object,
                    customersServiceMock.Object);

                expected = ImmutableList.Create(
                    new Customer {GlobalId = Guid.NewGuid()},
                    new Customer {GlobalId = Guid.NewGuid()}
                );

                customerRepositoryMock.Setup(x => x.GetAll())
                    .Returns(expected);
            }

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
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly CustomerToAdd customerToAdd;
            private readonly Customer expectedCustomer;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Mock<ICustomersService> customersServiceMock;

            private readonly CustomersController sut;

            public CreateShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                customersServiceMock = new Mock<ICustomersService>();
                var loggingMock = new Mock<ILogger<CustomersController>>();
                var getMobilesByCustomerIdQueryMock = new Mock<IGetMobilesByCustomerIdQuery>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                customerToAdd = new CustomerToAdd
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

                customersServiceMock.Setup(x => x.Create(customerToAdd))
                    .Returns(expectedCustomer);

                sut = new CustomersController(customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    getMobilesByCustomerIdQueryMock.Object,
                    customersServiceMock.Object);
            }

            [Fact]
            public void SendCustomerToCustomersService()
            {
                sut.Create(customerToAdd);

                customersServiceMock.Verify(x => x.Create(customerToAdd));
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
            private readonly OrderToAdd expectedOrderToAdd;
            private readonly Guid expectedCustomerGlobalId;
            private readonly Mobile expectedMobile;

            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<IMonitoring> monitoringMock;
            private readonly Mock<ICustomersService> customersServiceMock;

            private readonly CustomersController sut;

            public ProvisionShould()
            {
                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                monitoringMock = new Mock<IMonitoring>();
                customersServiceMock = new Mock<ICustomersService>();
                var loggingMock = new Mock<ILogger<CustomersController>>();
                var getMobilesByCustomerIdQueryMock = new Mock<IGetMobilesByCustomerIdQuery>();
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                expectedOrderToAdd = new OrderToAdd
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };
                expectedCustomerGlobalId = Guid.NewGuid();
                expectedMobile = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity()
                {
                    Id = 101,
                    GlobalId = Guid.NewGuid(),
                    CustomerId = expectedCustomerGlobalId,
                    State = Mobile.MobileState.New.ToString()
                });

                customersServiceMock.Setup(x => x.Provision(expectedCustomerGlobalId, expectedOrderToAdd))
                    .Returns(expectedMobile);

                sut = new CustomersController(customerRepositoryMock.Object,
                    mobileRepositoryMock.Object,
                    monitoringMock.Object,
                    getMobilesByCustomerIdQueryMock.Object, 
                    customersServiceMock.Object);
            }

            [Fact]
            public void SendOrderToCustomersService()
            {
                sut.Provision(expectedCustomerGlobalId, expectedOrderToAdd);

                customersServiceMock.Verify(x => x.Provision(expectedCustomerGlobalId , expectedOrderToAdd));
            }

            [Fact]
            public void ReturnOk()
            {
                var actual = sut.Provision(expectedCustomerGlobalId, expectedOrderToAdd);

                actual.Should().BeOfType<OkObjectResult>();
            }

            [Fact]
            public void ReturnNewMobile()
            {
                var actual = sut.Provision(expectedCustomerGlobalId, expectedOrderToAdd);
                var actualResult = actual as OkObjectResult;
                var actualMobile = actualResult.Value as MobileResource;

                actualMobile.Should().NotBeNull();
                actualMobile.GlobalId.Should().Be(expectedMobile.GlobalId);
                actualMobile.CustomerId.Should().Be(expectedCustomerGlobalId);
            }

            [Fact]
            public void ReturnNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();

                guidCreatorMock.Setup(x => x.Create()).Returns(notFoundGlobalId);

                var actual = sut.Provision(notFoundGlobalId, expectedOrderToAdd);

                actual.Should().BeOfType<NotFoundResult>();
            }
        }
    }
}