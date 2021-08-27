using System;
using System.Linq;
using FluentAssertions;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    namespace GetNewActivatesQuerySpec
    {
        [Collection("MobilesTests")]
        public class GetShould : IDisposable
        {
            private readonly MobilesSharedFixture fixture;

            public GetShould(MobilesSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
            }

            public void Dispose()
            {
                fixture.DataAccess.Cleanup();
            }

            [Fact]
            public void ReturnNewActivates()
            {
                var mobileBuilder = new MobileBuilder();
                var newActivate1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Activate)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newProvision1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newActivate2 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Activate)
                    .WithOrderState(Order.State.New)
                    .Build();
                fixture.DataAccess.Add(newActivate1);
                fixture.DataAccess.Add(newProvision1);
                fixture.DataAccess.Add(newActivate2);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewActivatesQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(newActivate1);
                actual.ElementAt(1).Should().BeEquivalentTo(newActivate2);
                actual.Count.Should().Be(2);
            }

            [Fact]
            public void ReturnEmpty_WhenNoMobiles()
            {
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewActivatesQuery(context, new DateTimeCreator());

                var actual = sut.Get();

                actual.Should().BeEmpty();
            }
        }
    }
}