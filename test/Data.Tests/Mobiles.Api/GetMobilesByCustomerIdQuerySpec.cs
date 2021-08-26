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
    namespace GetMobilesByCustomerIdQuerySpec
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
            public void ReturnMobiles()
            {
                var customerId = Guid.NewGuid();
                var mobile1 = new Mobile(new DateTimeCreator(), new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = customerId,
                    State = "New",
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>()
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            State = "New"
                        }
                    }
                });
                var mobile2 = new Mobile(new DateTimeCreator(), new MobileDataEntity()
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = customerId,
                    State = "New",
                    PhoneNumber = "0700000002",
                    Orders = new List<OrderDataEntity>()
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Buzz Aldrin",
                            ContactPhoneNumber = "0800000002",
                            State = "New"
                        }
                    }
                });
                using var context = new MobilesContext(fixture.ContextOptions);
                var mobilesRepository = new MobileRepository(context, new EnumConverter(), new DateTimeCreator());
                mobilesRepository.Add(mobile1);
                mobilesRepository.Add(mobile2);
                mobilesAdded.Add(mobile1);
                mobilesAdded.Add(mobile2);
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