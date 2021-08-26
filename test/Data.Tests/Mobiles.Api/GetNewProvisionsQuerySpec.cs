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
    namespace GetNewProvisionsQuerySpec
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
            public void ReturnNewProvisions()
            {
                var newProvision1 = new Mobile(new DateTimeCreator(), new MobileDataEntity
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
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            State = Order.State.New.ToString()
                        }
                    }
                });
                var newProvision2 = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = Mobile.MobileState.New.ToString(),
                    PhoneNumber = "0700000002",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Buzz Aldrin",
                            ContactPhoneNumber = "0800000002",
                            State = Order.State.New.ToString()
                        }
                    }
                });
                var newProvision3 = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = Mobile.MobileState.New.ToString(),
                    PhoneNumber = "0700000003",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Michael Collins",
                            ContactPhoneNumber = "0800000003",
                            State = Order.State.New.ToString()
                        }
                    }
                });
                fixture.DataAccess.Add(newProvision1);
                fixture.DataAccess.Add(newProvision2);
                fixture.DataAccess.Add(newProvision3);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewProvisionsQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(newProvision1);
                actual.ElementAt(1).Should().BeEquivalentTo(newProvision2);
                actual.ElementAt(2).Should().BeEquivalentTo(newProvision3);
            }

            [Theory]
            [InlineData("ProcessingProvision")]
            [InlineData("Live")]
            [InlineData("Ceased")]
            public void DoesNotReturnMobilesWhichAreNotNew(string state)
            {
                var newProvision1 = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = state,
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>
                    {
                        new()
                        {
                            GlobalId = Guid.NewGuid(),
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            State = Order.State.New.ToString()
                        }
                    }
                });
                fixture.DataAccess.Add(newProvision1);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewProvisionsQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void DoesNotReturnMobilesWhichHaveNoOrders()
            {
                var newProvision1 = new Mobile(new DateTimeCreator(), new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    CustomerId = Guid.NewGuid(),
                    State = Mobile.MobileState.New.ToString(),
                    PhoneNumber = "0700000001",
                    Orders = new List<OrderDataEntity>()
                });
                fixture.DataAccess.Add(newProvision1);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewProvisionsQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.Should().BeEmpty();
            }

            [Theory]
            [InlineData("Processing")]
            [InlineData("Sent")]
            [InlineData("Completed")]
            public void DoesNotReturnMobilesWithOrderWhichAreNotNew(string orderState)
            {
                var newProvision1 = new Mobile(new DateTimeCreator(), new MobileDataEntity
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
                            Name = "Neil Armstrong",
                            ContactPhoneNumber = "0800000001",
                            State = orderState,
                            Type = Order.OrderType.Provision.ToString(),
                        }
                    }
                });
                fixture.DataAccess.Add(newProvision1);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewProvisionsQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void ReturnEmpty_WhenNoMobiles()
            {
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewProvisionsQuery(context, new DateTimeCreator());

                var actual = sut.Get();

                actual.Should().BeEmpty();
            }
        }
    }
}