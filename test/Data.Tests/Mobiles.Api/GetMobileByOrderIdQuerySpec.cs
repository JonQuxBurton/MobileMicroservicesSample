using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Xunit;
using Xunit.Abstractions;

namespace Data.Tests.Mobiles.Api
{
    namespace GetMobileByOrderIdQuerySpec
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
            public void ReturnMobile()
            {
                var expectedMobile = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = Mobile.MobileState.New.ToString(),
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Type = Order.OrderType.Provision.ToString(),
                            State = Order.State.New.ToString()
                        }
                    }
                });
                fixture.DataAccess.Add(expectedMobile);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetMobileByOrderIdQuery(context, new DateTimeCreator());

                var actual = sut.Get(expectedMobile.Orders.First().GlobalId);

                actual.Should().BeEquivalentTo(expectedMobile);
            }

            [Fact]
            public void ReturnNull_WhenNotFound()
            {
                var notFoundOrderId = Guid.NewGuid();
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetMobileByOrderIdQuery(context, new DateTimeCreator());

                var actual = sut.Get(notFoundOrderId);

                actual.Should().BeNull();
            }
        }
    }
}