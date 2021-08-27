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
    namespace GetMobilesByCustomerIdQuerySpec
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
            public void ReturnMobiles()
            {
                var customerId = Guid.NewGuid();
                var mobileBuilder = new MobileBuilder();
                var mobile1 = mobileBuilder
                    .WithCustomerId(customerId)
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                var mobile2 = mobileBuilder
                    .WithCustomerId(customerId)
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                using var context = new MobilesContext(fixture.ContextOptions);
                fixture.DataAccess.Add(mobile1);
                fixture.DataAccess.Add(mobile2);
                var sut = new GetMobilesByCustomerIdQuery(context, new DateTimeCreator());

                var actual = sut.Get(customerId).ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(mobile1);
                actual.ElementAt(1).Should().BeEquivalentTo(mobile2);
            }

            [Fact]
            public void ReturnEmpty_WhenNoMobiles()
            {
                var notFoundCustomerId = Guid.NewGuid();
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetMobilesByCustomerIdQuery(context, new DateTimeCreator());

                var actual = sut.Get(notFoundCustomerId);

                actual.Should().BeEmpty();
            }
        }
    }
}