using System;
using System.Collections.Generic;
using FluentAssertions;
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
        [Collection("MobilesTests")]
        public class AddShould : IDisposable
        {
            private readonly MobilesSharedFixture fixture;
            private Mobile expectedMobile;
            private MobileRepository sut;

            public AddShould(MobilesSharedFixture fixture, ITestOutputHelper output)
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

        [Collection("MobilesTests")]
        public class UpdateShould : IDisposable
        {
            private readonly MobilesSharedFixture fixture;
            private Mobile expectedMobile;
            private MobileRepository sut;

            public UpdateShould(MobilesSharedFixture fixture, ITestOutputHelper output)
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

        [Collection("MobilesTests")]
        public class GetByIdUpdateShould : IDisposable
        {
            private readonly MobilesSharedFixture fixture;
            private readonly List<Mobile> mobiles;
            private MobileRepository sut;

            public GetByIdUpdateShould(MobilesSharedFixture fixture, ITestOutputHelper output)
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