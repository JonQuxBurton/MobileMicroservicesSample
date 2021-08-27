using System;
using FluentAssertions;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    namespace GetNextMobileIdQuerySpec
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
            public void ReturnNextMobileId()
            {
                var mobileBuilder = new MobileBuilder();
                var mobile = mobileBuilder
                    .WithMobileState(Mobile.MobileState.Live)
                    .WithOrderType(Order.OrderType.Activate)
                    .WithOrderState(Order.State.Completed)
                    .Build();
                fixture.DataAccess.Add(mobile);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNextMobileIdQuery(context);

                var actual = sut.Get();

                actual.Should().Be(mobile.Id + 1);
            }

            [Fact]
            public void ReturnOne_WhenNoOrders()
            {
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNextMobileIdQuery(context);

                var actual = sut.Get();

                actual.Should().Be(1);
            }
        }
    }
}