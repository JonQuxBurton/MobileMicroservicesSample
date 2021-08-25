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
    namespace GetNextMobileIdQuerySpec
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
            public void ReturnNextMobileId()
            {
                var mobile = new Mobile(new DateTimeCreator(), new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = "Live",
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>()
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
                var mobilesRepository = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());
                mobilesRepository.Add(mobile);
                mobilesAdded.Add(mobile);
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