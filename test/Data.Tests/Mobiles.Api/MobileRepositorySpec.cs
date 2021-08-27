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
                var mobileBuilder = new MobileBuilder();
                expectedMobile = mobileBuilder
                    .WithMobileState(Mobile.MobileState.Live)
                    .WithOrderType(Order.OrderType.Activate)
                    .WithOrderState(Order.State.Completed)
                    .Build();
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
                var mobileBuilder = new MobileBuilder();
                expectedMobile = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .BuildWithoutOrder();
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
                var mobileBuilder = new MobileBuilder();
                var expectedMobile = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .BuildWithoutOrder();
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