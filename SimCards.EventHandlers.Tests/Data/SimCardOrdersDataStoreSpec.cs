using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using SimCards.EventHandlers.Data;
using Xunit;
using Xunit.Abstractions;

namespace SimCards.EventHandlers.Tests.Data
{
    namespace SimCardOrdersDataStoreSpec
    {
        public class SimCardOrdersSharedFixture
        {
            public SimCardOrdersDataAccess SimCardOrdersDataAccess;
            
            public SimCardOrdersDataStore CreateSimCardOrdersDataStore()
            {
                return new(Options.Create(new Config
                {
                    ConnectionString = ConfigurationData.ConnectionString
                }));
            }

            private SimCardOrdersDataAccess CreateDataAccess(ITestOutputHelper output)
            {
                return new(output, ConfigurationData.ConnectionString);
            }

            public void Setup(ITestOutputHelper output)
            {
                SimCardOrdersDataAccess = CreateDataAccess(output);
            }
        }

        [CollectionDefinition("SimCardOrdersDataStoreSpec")]
        public class DatabaseCollection : ICollectionFixture<SimCardOrdersSharedFixture>
        { }

        [Collection("SimCardOrdersDataStoreSpec")]
        public class GetExistingShould : IDisposable
        {
            private readonly SimCardOrdersSharedFixture fixture;
            private readonly SimCardOrdersDataStore sut;

            public GetExistingShould(SimCardOrdersSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                sut = fixture.CreateSimCardOrdersDataStore();
            }

            [Fact]
            public void ReturnExistingSimCardOrder()
            {
                var expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = "New"
                };

                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder);

                var actual = sut.GetExisting(expectedSimCardOrder.MobileId, expectedSimCardOrder.MobileOrderId);

                actual.Should().BeEquivalentTo(expectedSimCardOrder, o => o.Excluding(x => x.CreatedAt));
            }

            [Fact]
            public void ReturnNull_WhenSimCardOrderNotFound()
            {
                var notFoundMobileId = Guid.NewGuid();
                var notFoundMobileOrderId = Guid.NewGuid();

                var actual = sut.GetExisting(notFoundMobileId, notFoundMobileOrderId);

                actual.Should().BeNull();
            }

            public void Dispose()
            {
                fixture.SimCardOrdersDataAccess.CleanUp();
            }
        }

        [Collection("SimCardOrdersDataStoreSpec")]
        public class GetSentShould : IDisposable
        {
            private readonly SimCardOrdersSharedFixture fixture;
            private readonly SimCardOrdersDataStore sut;

            public GetSentShould(SimCardOrdersSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                sut = fixture.CreateSimCardOrdersDataStore();
            }

            public void Dispose()
            {
                fixture.SimCardOrdersDataAccess.CleanUp();
            }

            [Fact]
            public void ReturnSentSimCardOrders()
            {
                var expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0700100001",
                    Status = "Sent"
                };
                var expectedSimCardOrder2 = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Buzz Aldrin",
                    PhoneNumber = "0700100002",
                    Status = "Sent"
                };
                var expectedSimCardOrder3 = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Michael Collins",
                    PhoneNumber = "0700100003",
                    Status = "Sent"
                };

                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder);
                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder2);
                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder3);

                var actual = sut.GetSent().ToList();

                actual.First().Should().BeEquivalentTo(expectedSimCardOrder, o => o.Excluding(x => x.CreatedAt));
                actual.ElementAt(1).Should().BeEquivalentTo(expectedSimCardOrder2, o => o.Excluding(x => x.CreatedAt));
                actual.ElementAt(2).Should().BeEquivalentTo(expectedSimCardOrder3, o => o.Excluding(x => x.CreatedAt));
            }


            [Fact]
            public void ReturnEmpty_WhenNoSimCardOrders()
            {
                var actual = sut.GetSent();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void ReturnEmpty_WhenNoSentSimCardOrders()
            {
                var expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = "New"
                };

                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder);

                var actual = sut.GetSent();

                actual.Should().BeEmpty();
            }
        }

        [Collection("SimCardOrdersDataStoreSpec")]
        public class AddShould : IDisposable
        {
            private readonly SimCardOrdersSharedFixture fixture;
            private SimCardOrder expectedSimCardOrder;

            public AddShould(SimCardOrdersSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            [Fact]
            public void AddSimCardOrder()
            {
                var sut = fixture.CreateSimCardOrdersDataStore();

                expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = "New"
                };

                using (sut.BeginTransaction())
                    sut.Add(expectedSimCardOrder);

                var actual = sut.GetExisting(expectedSimCardOrder.MobileId, expectedSimCardOrder.MobileOrderId);

                actual.Should().BeEquivalentTo(expectedSimCardOrder, o => o.Excluding(x => x.CreatedAt));
            }

            public void Dispose()
            {
                fixture.SimCardOrdersDataAccess.DeleteSimCardOrder(expectedSimCardOrder.MobileId);
                fixture.SimCardOrdersDataAccess.CleanUp();
            }
        }

        [Collection("SimCardOrdersDataStoreSpec")]
        public class SentShould : IDisposable
        {
            private readonly SimCardOrdersSharedFixture fixture;

            public SentShould(SimCardOrdersSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            [Fact]
            public void SetSimCardOrderToSent()
            {
                var sut = fixture.CreateSimCardOrdersDataStore();

                var expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = "New"
                };

                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder);

                using (sut.BeginTransaction()) 
                    sut.Sent(expectedSimCardOrder.MobileOrderId);

                var actual = sut.GetExisting(expectedSimCardOrder.MobileId, expectedSimCardOrder.MobileOrderId);

                actual.Status.Should().Be("Sent");
            }

            public void Dispose()
            {
                fixture.SimCardOrdersDataAccess.CleanUp();
            }
        }

        [Collection("SimCardOrdersDataStoreSpec")]
        public class CompleteShould : IDisposable
        {
            private readonly SimCardOrdersSharedFixture fixture;

            public CompleteShould(SimCardOrdersSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            [Fact]
            public void SetSimCardOrderToSent()
            {
                var sut = fixture.CreateSimCardOrdersDataStore();

                var expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = "Sent"
                };

                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder);

                using (var tx = sut.BeginTransaction())
                    sut.Complete(expectedSimCardOrder.MobileOrderId);

                var actual = sut.GetExisting(expectedSimCardOrder.MobileId, expectedSimCardOrder.MobileOrderId);

                actual.Status.Should().Be("Completed");
            }

            public void Dispose()
            {
                fixture.SimCardOrdersDataAccess.CleanUp();
            }
        }

        [Collection("SimCardOrdersDataStoreSpec")]
        public class IncrementAttemptsShould : IDisposable
        {
            private readonly SimCardOrdersSharedFixture fixture;

            public IncrementAttemptsShould(SimCardOrdersSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            [Fact]
            public void SetSimCardOrderToSent()
            {
                var sut = fixture.CreateSimCardOrdersDataStore();

                var expectedSimCardOrder = new SimCardOrder()
                {
                    MobileId = Guid.NewGuid(),
                    MobileOrderId = Guid.NewGuid(),
                    Name = "Neil Armstrong",
                    PhoneNumber = "0123456789",
                    Status = "New",
                    Attempts = 1
                };

                fixture.SimCardOrdersDataAccess.InsertSimCardOrder(expectedSimCardOrder);

                using (sut.BeginTransaction())
                    sut.IncrementAttempts(expectedSimCardOrder);

                var actual = sut.GetExisting(expectedSimCardOrder.MobileId, expectedSimCardOrder.MobileOrderId);

                actual.Attempts.Should().Be(2);
            }

            public void Dispose()
            {
                fixture.SimCardOrdersDataAccess.CleanUp();
            }
        }
    }
}
