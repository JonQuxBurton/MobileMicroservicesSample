using System;
using System.Linq;
using System.Threading;
using ExternalSimCardsProvider.Api.Configuration;
using ExternalSimCardsProvider.Api.Data;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.ExternalSimCardsProvider.Api
{
    namespace OrdersDataStoreSpec
    {
        public class OrdersDataStoreSharedFixture
        {
            public DataAccess DataAccess;

            public OrdersDataStore CreateDataStore()
            {
                return new(Options.Create(new Config
                {
                    ConnectionString = ConfigurationData.ConnectionString
                }));
            }

            private DataAccess CreateDataAccess(ITestOutputHelper output)
            {
                return new(output, ConfigurationData.ConnectionString);
            }

            public void Setup(ITestOutputHelper output)
            {
                DataAccess = CreateDataAccess(output);
            }
        }

        [CollectionDefinition("OrdersDataStoreSpec")]
        public class OrdersDataStoreSpecCollection : ICollectionFixture<OrdersDataStoreSharedFixture>
        {
        }

        [Collection("OrdersDataStoreSpec")]
        public class GetAllShould : IDisposable
        {
            private readonly OrdersDataStoreSharedFixture fixture;
            private readonly OrdersDataStore sut;

            public GetAllShould(OrdersDataStoreSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                sut = fixture.CreateDataStore();
            }

            public void Dispose()
            {
                fixture.DataAccess.CleanUp();
            }

            [Fact]
            public void ReturnAllOrders()
            {
                var order1 = new Order
                {
                    PhoneNumber = "0700000001",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };
                var order2 = new Order
                {
                    PhoneNumber = "0700000002",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };
                var order3 = new Order
                {
                    PhoneNumber = "0700000003",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };

                fixture.DataAccess.InsertOrder(order1);
                Thread.Sleep(10);
                fixture.DataAccess.InsertOrder(order2);
                Thread.Sleep(10);
                fixture.DataAccess.InsertOrder(order3);

                var actual = sut.GetAll().ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(order3, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
                actual.ElementAt(1).Should().BeEquivalentTo(order2, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
                actual.ElementAt(2).Should().BeEquivalentTo(order1, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
            }

            [Fact]
            public void ReturnEmpty_WhenNoOrders()
            {
                var actual = sut.GetAll();

                actual.Should().BeEmpty();
            }
        }

        [Collection("OrdersDataStoreSpec")]
        public class GetByReferenceShould : IDisposable
        {
            private readonly OrdersDataStoreSharedFixture fixture;
            private readonly OrdersDataStore sut;

            public GetByReferenceShould(OrdersDataStoreSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                sut = fixture.CreateDataStore();
            }

            public void Dispose()
            {
                fixture.DataAccess.CleanUp();
            }

            [Fact]
            public void ReturnExistingOrder()
            {
                var expectedOrder = new Order
                {
                    PhoneNumber = "0700000001",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };

                fixture.DataAccess.InsertOrder(expectedOrder);

                var actual = sut.GetByReference(expectedOrder.Reference);

                actual.Should().BeEquivalentTo(expectedOrder, o => o
                    .Excluding(x => x.CreatedAt)
                    .Excluding(x => x.Id));
            }

            [Fact]
            public void ReturnNull_WhenOrderNotFound()
            {
                var notReference = Guid.NewGuid();

                var actual = sut.GetByReference(notReference);

                actual.Should().BeNull();
            }
        }        
        
        [Collection("OrdersDataStoreSpec")]
        public class GetMaxIdShould : IDisposable
        {
            private readonly OrdersDataStoreSharedFixture fixture;
            private readonly OrdersDataStore sut;

            public GetMaxIdShould(OrdersDataStoreSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                sut = fixture.CreateDataStore();
            }

            public void Dispose()
            {
                fixture.DataAccess.CleanUp();
            }

            [Fact]
            public void ReturnMaxId()
            {
                var expectedOrder = new Order
                {
                    PhoneNumber = "0700000001",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };

                fixture.DataAccess.InsertOrder(expectedOrder);
                var previousOrder = sut.GetByReference(expectedOrder.Reference);

                var actual = sut.GetMaxId();

                actual.Should().Be(previousOrder.Id);
            }

            [Fact]
            public void ReturnZero_WhenNoOrders()
            {
                var actual = sut.GetMaxId();

                actual.Should().Be(0);
            }
        }

        [Collection("OrdersDataStoreSpec")]
        public class AddShould : IDisposable
        {
            private readonly OrdersDataStoreSharedFixture fixture;
            private readonly OrdersDataStore sut;
            private Order expectedOrder;

            public AddShould(OrdersDataStoreSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                sut = fixture.CreateDataStore();
            }

            public void Dispose()
            {
                fixture.DataAccess.DeleteOrder(expectedOrder.Reference);
                fixture.DataAccess.CleanUp();
            }

            [Fact]
            public void AddOrder()
            {
                expectedOrder = new Order
                {
                    PhoneNumber = "0700000001",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };
                
                using (sut.BeginTransaction())
                    sut.Add(expectedOrder);

                var actual = sut.GetByReference(expectedOrder.Reference);

                actual.Should().BeEquivalentTo(expectedOrder, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
            }
        }

        [Collection("OrdersDataStoreSpec")]
        public class CompleteShould : IDisposable
        {
            private readonly OrdersDataStoreSharedFixture fixture;

            public CompleteShould(OrdersDataStoreSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            [Fact]
            public void SetOrderToCompleted()
            {
                var sut = fixture.CreateDataStore();

                var expectedOrder = new Order
                {
                    PhoneNumber = "0700000001",
                    Reference = Guid.NewGuid(),
                    Status = "New"
                };

                fixture.DataAccess.InsertOrder(expectedOrder);

                using (var tx = sut.BeginTransaction())
                    sut.Complete(expectedOrder);

                var actual = sut.GetByReference(expectedOrder.Reference);

                actual.Status.Should().Be("Completed");
            }

            public void Dispose()
            {
                fixture.DataAccess.CleanUp();
            }
        }
    }
}