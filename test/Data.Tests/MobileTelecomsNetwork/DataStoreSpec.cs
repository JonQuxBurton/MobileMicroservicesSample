using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MobileTelecomsNetwork.EventHandlers;
using MobileTelecomsNetwork.EventHandlers.Data;
using MobileTelecomsNetwork.EventHandlers.Domain;
using Utils.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.MobileTelecomsNetwork
{
    namespace DataStoreSpec
    {
        public class DataStoreSharedFixture
        {
            public DataAccess DataAccess;

            public DataStore CreateDataStore()
            {
                return new(Options.Create(new Config
                {
                    ConnectionString = ConfigurationData.ConnectionString
                }), new EnumConverter());
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

        [CollectionDefinition("DataStoreSpec")]
        public class DataStoreSpecCollection : ICollectionFixture<DataStoreSharedFixture>
        { }

        [Collection("DataStoreSpec")]
        public class DataStoreShould : IDisposable
        {
            private readonly DataStoreSharedFixture fixture;
            private Order expectedOrder;

            public DataStoreShould(DataStoreSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            public void Dispose()
            {
                fixture.DataAccess.DeleteOrder(expectedOrder.MobileOrderId);
            }

            [Fact]
            public void AddOrder()
            {
                var sut = fixture.CreateDataStore();
                expectedOrder = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Type = OrderType.Activate,
                    Status = OrderStatus.New
                };

                using (sut.BeginTransaction())
                    sut.Add(expectedOrder);

                var actual = fixture.DataAccess.GetOrderById(expectedOrder.MobileOrderId);
                actual.Should().BeEquivalentTo(expectedOrder, o => o.Excluding(x => x.CreatedAt));
            }
            
            [Fact]
            public void SetOrderToSent()
            {
                var sut = fixture.CreateDataStore();
                expectedOrder = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Type = OrderType.Activate,
                    Status = OrderStatus.New,
                };
                using (sut.BeginTransaction())
                    sut.Add(expectedOrder);

                sut.Sent(expectedOrder.MobileOrderId);

                var actual = fixture.DataAccess.GetOrderById(expectedOrder.MobileOrderId);
                actual.Status.Should().Be(OrderStatus.Sent);
            }
            
            [Fact]
            public void SetOrderToCompleted()
            {
                var sut = fixture.CreateDataStore();
                expectedOrder = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Type = OrderType.Activate,
                    Status = OrderStatus.Sent
                };
                using (sut.BeginTransaction())
                    sut.Add(expectedOrder);

                sut.Complete(expectedOrder.MobileOrderId);

                var actual = fixture.DataAccess.GetOrderById(expectedOrder.MobileOrderId);
                actual.Status.Should().Be(OrderStatus.Completed);
            }
            
            [Fact]
            public void IncrementAttempts()
            {
                var sut = fixture.CreateDataStore();
                expectedOrder = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Type = OrderType.Activate,
                    Status = OrderStatus.New
                };
                using (sut.BeginTransaction())
                    sut.Add(expectedOrder);

                sut.IncrementAttempts(expectedOrder);

                var actual = fixture.DataAccess.GetOrderById(expectedOrder.MobileOrderId);
                actual.Attempts.Should().Be(1);
            }
        }

        [Collection("DataStoreSpec")]
        public class DataStore_GetSent_Should : IDisposable
        {
            private readonly DataStoreSharedFixture fixture;
            private DataStore sut;

            public DataStore_GetSent_Should(DataStoreSharedFixture fixture, ITestOutputHelper output)
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
            public void ReturnSentOrders()
            {
                var expectedOrder1 = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0700100001",
                    Status = OrderStatus.Sent
                };
                var completedOrder = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Buzz Aldrin",
                    PhoneNumber = "0700100002",
                    Status = OrderStatus.Completed
                };
                var expectedOrder2 = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Michael Collins",
                    PhoneNumber = "0700100003",
                    Status = OrderStatus.Sent
                };

                fixture.DataAccess.InsertOrder(expectedOrder1);
                fixture.DataAccess.InsertOrder(completedOrder);
                fixture.DataAccess.InsertOrder(expectedOrder2);

                var actual = sut.GetSent().ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(expectedOrder1, o => o.Excluding(x => x.CreatedAt));
                actual.ElementAt(1).Should().BeEquivalentTo(expectedOrder2, o => o.Excluding(x => x.CreatedAt));
            }

            [Fact]
            public void ReturnEmpty_WhenOrders()
            {
                var actual = sut.GetSent();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void ReturnEmpty_WhenNoSentOrders()
            {
                var expectedOrder = new Order()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = OrderStatus.New
                };
                fixture.DataAccess.InsertOrder(expectedOrder);

                var actual = sut.GetSent();

                actual.Should().BeEmpty();
            }
        }
    }
}
