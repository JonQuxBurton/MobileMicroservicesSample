using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    namespace CustomerRepositorySpec
    {
        public class CustomersRepositorySharedFixture
        {
            public CustomersDataAccess DataAccess;

            public DbContextOptions<MobilesContext>
                ContextOptions =>
                new DbContextOptionsBuilder<MobilesContext>()
                    .UseSqlServer(ConfigurationData.ConnectionString)
                    .Options;

            private CustomersDataAccess CreateDataAccess(ITestOutputHelper output)
            {
                return new(output, ConfigurationData.ConnectionString);
            }

            public void Setup(ITestOutputHelper output)
            {
                DataAccess = CreateDataAccess(output);
            }
        }

        [CollectionDefinition("CustomerRepositorySpec")]
        public class CustomerRepositorySpecCollection : ICollectionFixture<CustomersRepositorySharedFixture>
        {
        }

        [Collection("CustomerRepositorySpec")]
        public class AddShould : IDisposable
        {
            private readonly CustomersRepositorySharedFixture fixture;
            private Customer expectedCustomer;
            private CustomerRepository sut;

            public AddShould(CustomersRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            public void Dispose()
            {
                fixture.DataAccess.Delete(expectedCustomer.GlobalId);
            }

            [Fact]
            public void AddCustomer()
            {
                expectedCustomer = new Customer
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Neil Armstrong"
                };
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new CustomerRepository(context);

                sut.Add(expectedCustomer);

                var actual = sut.GetById(expectedCustomer.GlobalId);
                actual.Should().BeEquivalentTo(expectedCustomer);
            }
        }
        
        [Collection("CustomerRepositorySpec")]
        public class GetAllShould : IDisposable
        {
            private readonly CustomersRepositorySharedFixture fixture;
            private CustomerRepository sut;
            private readonly List<Customer> customers;

            public GetAllShould(CustomersRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
                customers = new List<Customer>();
            }

            public void Dispose()
            {
                foreach (var customer in customers)
                {
                    fixture.DataAccess.Delete(customer.GlobalId);
                }
            }

            [Fact]
            public void ReturnAllCustomers()
            {
                var expectedCustomer1 = new Customer
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Neil Armstrong"
                };
                var expectedCustomer2 = new Customer
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Buzz Aldrin"
                };
                var expectedCustomer3 = new Customer
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Michael Collins"
                };
                customers.Add(expectedCustomer1);
                customers.Add(expectedCustomer2);
                customers.Add(expectedCustomer3);
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new CustomerRepository(context);

                sut.Add(expectedCustomer1);
                sut.Add(expectedCustomer2);
                sut.Add(expectedCustomer3);

                var actual = sut.GetAll();

                actual.ElementAt(0).Should().BeEquivalentTo(expectedCustomer1);
                actual.ElementAt(1).Should().BeEquivalentTo(expectedCustomer2);
                actual.ElementAt(2).Should().BeEquivalentTo(expectedCustomer3);
            }
            
            [Fact]
            public void ReturnEmpty_WhenNoCustomers()
            {
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new CustomerRepository(context);

                var actual = sut.GetAll();

                actual.Should().BeEmpty();
            }
        }

        [Collection("CustomerRepositorySpec")]
        public class GetByIdShould : IDisposable
        {
            private readonly CustomersRepositorySharedFixture fixture;
            private CustomerRepository sut;
            private readonly List<Customer> customers;

            public GetByIdShould(CustomersRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
                customers = new List<Customer>();
            }

            public void Dispose()
            {
                foreach (var customer in customers) 
                    fixture.DataAccess.Delete(customer.GlobalId);
            }

            [Fact]
            public void ReturnCustomer()
            {
                var expectedCustomer1 = new Customer
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Neil Armstrong"
                };
                customers.Add(expectedCustomer1);
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new CustomerRepository(context);
                sut.Add(expectedCustomer1);

                var actual = sut.GetById(expectedCustomer1.GlobalId);

                actual.Should().BeEquivalentTo(expectedCustomer1);
            }

            [Fact]
            public void ReturnNull_WhenNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();

                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new CustomerRepository(context);

                var actual = sut.GetById(notFoundGlobalId);

                actual.Should().BeNull();
            }
        }

        [Collection("CustomerRepositorySpec")]
        public class UpdateShould : IDisposable
        {
            private readonly CustomersRepositorySharedFixture fixture;
            private CustomerRepository sut;
            private readonly List<Customer> customers;

            public UpdateShould(CustomersRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
                customers = new List<Customer>();
            }

            public void Dispose()
            {
                foreach (var customer in customers) 
                    fixture.DataAccess.Delete(customer.GlobalId);
            }

            [Fact]
            public void UpdateCustomer()
            {
                var expectedCustomer = new Customer
                {
                    GlobalId = Guid.NewGuid(),
                    Name = "Neil Armstrong"
                };
                customers.Add(expectedCustomer);
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new CustomerRepository(context);
                sut.Add(expectedCustomer);

                expectedCustomer.Name = "Tim Peake";
                sut.Update(expectedCustomer);

                var actual = sut.GetById(expectedCustomer.GlobalId);
                actual.Name.Should().Be("Tim Peake");
            }
        }
    }
}