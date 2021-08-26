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
                var newActivate1 = new Mobile(new DateTimeCreator(), new MobileDataEntity
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
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            Type = Order.OrderType.Activate.ToString(),
                            State = Order.State.New.ToString()
                        }
                    }
                });
                var newProvision1 = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = "New",
                    PhoneNumber = "0700000002",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Buzz Aldrin",
                            ContactPhoneNumber = "0800000002",
                            Type = Order.OrderType.Provision.ToString(),
                            State = Order.State.New.ToString()
                        }
                    }
                });
                var newActivate2 = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = "New",
                    PhoneNumber = "0700000003",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Michael Collins",
                            ContactPhoneNumber = "0800000003",
                            Type = Order.OrderType.Activate.ToString(),
                            State = Order.State.New.ToString()
                        }
                    }
                });
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