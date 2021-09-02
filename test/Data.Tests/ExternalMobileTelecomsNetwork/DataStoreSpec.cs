using System;
using System.Linq;
using System.Threading;
using ExternalMobileTelecomsNetwork.Api.Configuration;
using ExternalMobileTelecomsNetwork.Api.Data;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.ExternalMobileTelecomsNetwork
{
    namespace DataStoreSpec
    {
        public class ExternalMobileTelecomsNetworkDataStoreSharedFixture
        {
            public DataAccess DataAccess;

            public DataStore CreateDataStore()
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

        [CollectionDefinition("ExternalMobileTelecomsNetwork_DataStoreSpec")]
        public class
            ExternalMobileTelecomsNetworkDataStoreSpecCollection : ICollectionFixture<
                ExternalMobileTelecomsNetworkDataStoreSharedFixture>
        {
        }

        [Collection("ExternalMobileTelecomsNetwork_DataStoreSpec")]
        public class DataStoreShould : IDisposable
        {
            private readonly ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture;
            private Order expectedOrder;
            private readonly DataStore sut;
            private ActivationCode expectedActivationCode;

            public DataStoreShould(ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture,
                ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
                sut = fixture.CreateDataStore();
            }

            public void Dispose()
            {
                fixture.DataAccess.DeleteOrder(expectedOrder);
                fixture.DataAccess.DeleteActivationCode(expectedActivationCode);
            }

            [Fact]
            public void AddOrder()
            {
                expectedOrder = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0123456789",
                    ActivationCode = "ABC001"
                };

                using (sut.BeginTransaction())
                {
                    sut.Add(expectedOrder);
                }

                var actual = sut.GetByReference(expectedOrder.Reference);
                actual.Should().BeEquivalentTo(expectedOrder, o => o.Excluding(x => x.CreatedAt));
            }

            [Fact]
            public void SetOrderToCompleted()
            {
                expectedOrder = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0123456789",
                    ActivationCode = "ABC001"
                };
                using (sut.BeginTransaction())
                {
                    sut.Add(expectedOrder);
                }

                sut.Complete(expectedOrder.Reference);

                var actual = sut.GetByReference(expectedOrder.Reference);
                actual.Status.Should().Be("Completed");
            }

            [Fact]
            public void SetOrderToRejected()
            {
                var expectedReason = "RejectionReason";
                expectedOrder = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0123456789",
                    ActivationCode = "ABC001"
                };
                using (sut.BeginTransaction())
                {
                    sut.Add(expectedOrder);
                }

                sut.Reject(expectedOrder.Reference, expectedReason);

                var actual = sut.GetByReference(expectedOrder.Reference);
                actual.Status.Should().Be("Rejected");
                actual.Reason.Should().Be(expectedReason);
            }

            [Fact]
            public void AddCeaseOrder()
            {
                var expectedPhoneNumber = "0123456789";
                var expectedReference = Guid.NewGuid();

                using (sut.BeginTransaction())
                {
                    sut.Cease(expectedPhoneNumber, expectedReference);
                }

                var actual = sut.GetByReference(expectedReference);
                actual.Reference.Should().Be(expectedReference);
                actual.Type.Should().BeEquivalentTo("Cease");
                actual.Status.Should().BeEquivalentTo("New");
                actual.PhoneNumber.Should().BeEquivalentTo(expectedPhoneNumber);

                expectedOrder = actual;
            }
            
            [Fact]
            public void AddActivationCode()
            {
                expectedActivationCode = new ActivationCode
                {
                    Code = "ABC123",
                    PhoneNumber = "0123456789"
                };

                using (sut.BeginTransaction()) 
                    sut.InsertActivationCode(expectedActivationCode);

                var actual = sut.GetActivationCode(expectedActivationCode.PhoneNumber);
                actual.Should().BeEquivalentTo(expectedActivationCode, o => o.Excluding(x => x.CreatedAt));
            }

            [Fact]
            public void UpdateActivationCode()
            {
                var newActivationCode = "XYZ456";
                expectedActivationCode = new ActivationCode
                {
                    Code = "ABC123",
                    PhoneNumber = "0123456789"
                };
                using (sut.BeginTransaction())
                    sut.InsertActivationCode(expectedActivationCode);
                expectedActivationCode.Code = newActivationCode;

                using (sut.BeginTransaction())
                    sut.UpdateActivationCode(expectedActivationCode);

                var actual = sut.GetActivationCode(expectedActivationCode.PhoneNumber);
                actual.Code.Should().Be(newActivationCode);
            }
        }

        [Collection("ExternalMobileTelecomsNetwork_DataStoreSpec")]
        public class DataStore_GetAll_Should : IDisposable
        {
            private readonly ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture;
            private readonly DataStore sut;

            public DataStore_GetAll_Should(ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture,
                ITestOutputHelper output)
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
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000001",
                    ActivationCode = "AAA001"
                };
                var order2 = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000002",
                    ActivationCode = "AAA002"
                };
                var order3 = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000003",
                    ActivationCode = "AAA003"
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
        
        [Collection("ExternalMobileTelecomsNetwork_DataStoreSpec")]
        public class DataStore_GetByReference_Should : IDisposable
        {
            private readonly ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture;
            private readonly DataStore sut;

            public DataStore_GetByReference_Should(ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture,
                ITestOutputHelper output)
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
            public void ReturnOrder()
            {
                var order1 = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000001",
                    ActivationCode = "AAA001"
                };
                var expectedOrder = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000002",
                    ActivationCode = "AAA002"
                };

                fixture.DataAccess.InsertOrder(order1);
                Thread.Sleep(10);
                fixture.DataAccess.InsertOrder(expectedOrder);

                var actual = sut.GetByReference(expectedOrder.Reference);

                actual.Should().BeEquivalentTo(expectedOrder, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
            }

            [Fact]
            public void ReturnNull_WhenNotFound()
            {
                var notFoundReference = Guid.NewGuid();

                var actual = sut.GetByReference(notFoundReference);

                actual.Should().BeNull();
            }
        }
        
        [Collection("ExternalMobileTelecomsNetwork_DataStoreSpec")]
        public class DataStore_GetByPhoneNumber_Should : IDisposable
        {
            private readonly ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture;
            private readonly DataStore sut;

            public DataStore_GetByPhoneNumber_Should(ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture,
                ITestOutputHelper output)
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
            public void ReturnOrder_WhenPhoneNumberAndStatusMatch()
            {
                var order1 = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000001",
                    ActivationCode = "AAA001"
                };
                var expectedOrder = new Order
                {
                    Reference = Guid.NewGuid(),
                    Type = "Activate",
                    Status = "New",
                    PhoneNumber = "0700000002",
                    ActivationCode = "AAA002"
                };

                fixture.DataAccess.InsertOrder(order1);
                Thread.Sleep(10);
                fixture.DataAccess.InsertOrder(expectedOrder);

                var actual = sut.GetByPhoneNumber(expectedOrder.PhoneNumber, expectedOrder.Status);

                actual.Should().BeEquivalentTo(expectedOrder, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
            }

            [Fact]
            public void ReturnNull_WhenNotFound()
            {
                var notFoundPhoneNumber = "07000000001";

                var actual = sut.GetByPhoneNumber(notFoundPhoneNumber, "New");

                actual.Should().BeNull();
            }
        }        
        
        [Collection("ExternalMobileTelecomsNetwork_DataStoreSpec")]
        public class DataStore_GetActivationCode_Should : IDisposable
        {
            private readonly ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture;
            private readonly DataStore sut;

            public DataStore_GetActivationCode_Should(ExternalMobileTelecomsNetworkDataStoreSharedFixture fixture,
                ITestOutputHelper output)
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
            public void ReturnOrder_WhenPhoneNumberAndStatusMatch()
            {
                var activationCode1 = new ActivationCode
                {
                    PhoneNumber = "0700000001",
                    Code = "ABC001"
                };
                var expectedActivationCode = new ActivationCode
                {
                    PhoneNumber = "0700000002",
                    Code = "XYZ002"
                };

                fixture.DataAccess.InsertActivationCode(activationCode1);
                Thread.Sleep(10);
                fixture.DataAccess.InsertActivationCode(expectedActivationCode);

                using var transaction = sut.BeginTransaction();
                var actual = sut.GetActivationCode(expectedActivationCode.PhoneNumber);
                actual.Should().BeEquivalentTo(expectedActivationCode, o => o.Excluding(x => x.CreatedAt).Excluding(x => x.Id));
            }

            [Fact]
            public void ReturnNull_WhenNotFound()
            {
                var notFoundPhoneNumber = "07000000001";

                using var transaction = sut.BeginTransaction();
                var actual = sut.GetActivationCode(notFoundPhoneNumber);
                actual.Should().BeNull();
            }
        }
    }
}