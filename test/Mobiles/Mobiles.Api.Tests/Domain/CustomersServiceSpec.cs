using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using Moq;
using Utils.DateTimes;
using Utils.DomainDrivenDesign;
using Utils.Guids;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace CustomersServiceSpec
    {
        public class CreateShould
        {
            private readonly Customer expectedCustomer;
            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<ILogger<CustomersService>> loggerMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            private readonly CustomersService sut;

            public CreateShould()
            {
                expectedCustomer = new Customer()
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Neil Armstrong"
                };

                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                loggerMock = new Mock<ILogger<CustomersService>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                guidCreatorMock.Setup(x => x.Create()).Returns(expectedCustomer.GlobalId);
                customerRepositoryMock.Setup(x => x.GetById(expectedCustomer.GlobalId)).Returns(expectedCustomer);

                sut = new CustomersService(loggerMock.Object, 
                    guidCreatorMock.Object, 
                    dateTimeCreatorMock.Object,
                    customerRepositoryMock.Object,
                    mobileRepositoryMock.Object);
            }

            [Fact]
            public void ReturnCreatedCustomer()
            {
                var actual = sut.Create(new CustomerToAdd()
                {
                    Name = expectedCustomer.Name
                });

                actual.GlobalId.Should().Be(expectedCustomer.GlobalId);
                actual.Name.Should().Be(expectedCustomer.Name);
            }

            [Fact]
            public void AddCustomerToRepository()
            {
                sut.Create(new CustomerToAdd()
                {
                    Name = expectedCustomer.Name
                });

                customerRepositoryMock.Verify(x => x.Add(It.Is<Customer>(y =>
                    y.GlobalId == expectedCustomer.GlobalId &&
                    y.Name == expectedCustomer.Name)));
            }
        }

        public class ProvisionShould
        {
            private readonly Customer expectedCustomer;
            private readonly Guid expectedNewMobileGlobalId;

            private readonly Mock<ICustomerRepository> customerRepositoryMock;
            private readonly Mock<IRepository<Mobile>> mobileRepositoryMock;
            private readonly Mock<ILogger<CustomersService>> loggerMock;
            private readonly Mock<IGuidCreator> guidCreatorMock;
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            private readonly CustomersService sut;

            public ProvisionShould()
            {
                expectedCustomer = new Customer
                {
                    Name = "Armstrong Corporation",
                    GlobalId = Guid.NewGuid()
                };
                expectedNewMobileGlobalId = Guid.NewGuid();

                customerRepositoryMock = new Mock<ICustomerRepository>();
                mobileRepositoryMock = new Mock<IRepository<Mobile>>();
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
                loggerMock = new Mock<ILogger<CustomersService>>();
                guidCreatorMock = new Mock<IGuidCreator>();
                guidCreatorMock.SetupSequence(x => x.Create())
                    .Returns(expectedNewMobileGlobalId);
                customerRepositoryMock.Setup(x => x.GetById(expectedCustomer.GlobalId)).Returns(expectedCustomer);

                sut = new CustomersService(loggerMock.Object,
                    guidCreatorMock.Object,
                    dateTimeCreatorMock.Object,
                    customerRepositoryMock.Object,
                    mobileRepositoryMock.Object);
            }

            [Fact]
            public void AddMobileToRepositoryWithStateOfNew()
            {
                var expectedOrder = new OrderToAdd
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };

                sut.Provision(expectedCustomer.GlobalId, expectedOrder);

                mobileRepositoryMock.Verify(x => x.Add(It.Is<Mobile>(y =>
                    y.GlobalId == expectedNewMobileGlobalId &&
                    y.State == Mobile.MobileState.New &&
                    y.CustomerId == expectedCustomer.GlobalId)));
            }

            [Fact]
            public void AddMobileToRepositoryWithInFlightOrder()
            {
                var expectedOrder = new OrderToAdd
                {
                    Name = "Neil Armstrong",
                    ContactPhoneNumber = "01234 123123"
                };

                sut.Provision(expectedCustomer.GlobalId, expectedOrder);

                mobileRepositoryMock.Verify(x => x.Add(It.Is<Mobile>(y =>
                    y.GlobalId == expectedNewMobileGlobalId &&
                    y.State == Mobile.MobileState.New &&
                    y.InProgressOrder != null &&
                    y.InProgressOrder.CurrentState == Order.State.New)));
            }

            [Fact]
            public void ReturnNewMobile()
            {
                var actual = sut.Provision(expectedCustomer.GlobalId, new OrderToAdd());

                actual.GlobalId.Should().Be(expectedNewMobileGlobalId);
                actual.State.Should().Be(Mobile.MobileState.New);
                actual.InProgressOrder.Should().NotBeNull();
                actual.InProgressOrder.CurrentState.Should().Be(Order.State.New);
            }

            [Fact]
            public void ReturnNullWhenCustomerNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();

                guidCreatorMock.Setup(x => x.Create()).Returns(notFoundGlobalId);

                var actual = sut.Provision(notFoundGlobalId, new OrderToAdd());

                actual.Should().BeNull();
            }
        }

    }
}
