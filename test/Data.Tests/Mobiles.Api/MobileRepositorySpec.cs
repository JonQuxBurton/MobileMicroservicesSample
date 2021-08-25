using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Utils.Enums;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    namespace MobileRepositorySpec
    {
        public class MobileRepositorySharedFixture
        {
            public MobilesDataAccess DataAccess;

            public DbContextOptions<MobilesContext>
                ContextOptions =>
                new DbContextOptionsBuilder<MobilesContext>()
                    .UseSqlServer(ConfigurationData.ConnectionString)
                    .Options;

            private MobilesDataAccess CreateDataAccess(ITestOutputHelper output)
            {
                return new(output, ConfigurationData.ConnectionString);
            }

            public void Setup(ITestOutputHelper output)
            {
                DataAccess = CreateDataAccess(output);
            }
        }

        [CollectionDefinition("MobileRepositorySpec")]
        public class MobileRepositorySpecCollection : ICollectionFixture<MobileRepositorySharedFixture>
        {
        }

        [Collection("MobileRepositorySpec")]
        public class AddShould : IDisposable
        {
            private readonly MobileRepositorySharedFixture fixture;
            private Mobile expectedMobile;
            private MobileRepository sut;

            public AddShould(MobileRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            public void Dispose()
            {
                fixture.DataAccess.Delete(expectedMobile);
            }

            [Fact]
            public void AddMobile()
            {
                expectedMobile = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = "Live",
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            State = "Completed",
                            Type = "Activate"
                        }
                    }
                });
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());

                sut.Add(expectedMobile);

                var actual = sut.GetById(expectedMobile.GlobalId);
                actual.Should().BeEquivalentTo(expectedMobile);
            }
        }

        [Collection("MobileRepositorySpec")]
        public class UpdateShould : IDisposable
        {
            private readonly MobileRepositorySharedFixture fixture;
            private Mobile expectedMobile;
            private MobileRepository sut;

            public UpdateShould(MobileRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            public void Dispose()
            {
                fixture.DataAccess.Delete(expectedMobile);
            }

            [Fact]
            public void AddMobile()
            {
                expectedMobile = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = "New",
                    PhoneNumber = "0700000001"
                });
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());
                sut.Add(expectedMobile);

                expectedMobile.Provision();
                sut.Update(expectedMobile);

                var actual = sut.GetById(expectedMobile.GlobalId);
                actual.State.Should().Be(Mobile.MobileState.ProcessingProvision);
            }
        }

        [Collection("MobileRepositorySpec")]
        public class GetByIdUpdateShould : IDisposable
        {
            private readonly MobileRepositorySharedFixture fixture;
            private readonly List<Mobile> mobiles;
            private MobileRepository sut;

            public GetByIdUpdateShould(MobileRepositorySharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
                mobiles = new List<Mobile>();
            }

            public void Dispose()
            {
                foreach (var mobile in mobiles)
                    fixture.DataAccess.Delete(mobile);
            }

            [Fact]
            public void ReturnMobile()
            {
                var expectedMobile = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = "New",
                    PhoneNumber = "0700000001"
                });
                mobiles.Add(expectedMobile);
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());
                sut.Add(expectedMobile);

                var actual = sut.GetById(expectedMobile.GlobalId);

                actual.Should().BeEquivalentTo(expectedMobile);
            }

            [Fact]
            public void ReturnNull_WhenNotFound()
            {
                var notFoundGlobalId = Guid.NewGuid();
                using var context = new MobilesContext(fixture.ContextOptions);
                sut = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());

                var actual = sut.GetById(notFoundGlobalId);

                actual.Should().BeNull();
            }
        }
    }
}