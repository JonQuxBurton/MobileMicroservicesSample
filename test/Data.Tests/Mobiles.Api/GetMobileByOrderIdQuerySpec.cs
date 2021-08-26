using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Utils.Enums;
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
            private readonly List<Mobile> mobilesAdded;

            public GetShould(MobilesSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);

                mobilesAdded = new List<Mobile>();
            }

            public void Dispose()
            {
                foreach (var mobile in mobilesAdded)
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
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Type = "Provision",
                            State = "New"
                        }
                    }
                });
                using var context = new MobilesContext(fixture.ContextOptions);
                var mobilesRepository = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());
                mobilesRepository.Add(expectedMobile);
                mobilesAdded.Add(expectedMobile);
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