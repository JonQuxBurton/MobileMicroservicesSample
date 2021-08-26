using System;
using System.Collections.Generic;
using FluentAssertions;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
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
                var mobile = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = Mobile.MobileState.Live.ToString(),
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            State = Order.State.Completed.ToString(),
                            Type = Order.OrderType.Activate.ToString()
                        }
                    }
                });
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