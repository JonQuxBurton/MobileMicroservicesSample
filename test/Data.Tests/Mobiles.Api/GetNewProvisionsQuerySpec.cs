using System;
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
    namespace GetNewProvisionsQuerySpec
    {
        [Collection("MobilesTests")]
        public class GetShould : IDisposable
        {
            private readonly MobilesSharedFixture fixture;
            private readonly MobileBuilder mobileBuilder;

            public GetShould(MobilesSharedFixture fixture, ITestOutputHelper output)
            {
                this.fixture = fixture;
                this.fixture.Setup(output);
                mobileBuilder = new MobileBuilder();
            }

            public void Dispose()
            {
                fixture.DataAccess.Cleanup();
            }

            [Fact]
            public void ReturnNewProvisions()
            {
                var newProvision1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newProvision2 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newProvision3 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
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
            public void DoesNotReturnMobilesWhichAreNotNew(string stateString)
            {
                var state = new EnumConverter().ToEnum<Mobile.MobileState>(stateString);
                var newProvision1 = mobileBuilder
                    .WithMobileState(state)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                fixture.DataAccess.Add(newProvision1);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewProvisionsQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.Should().BeEmpty();
            }

            [Fact]
            public void DoesNotReturnMobilesWhichHaveNoOrders()
            {
                var newProvision1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .BuildWithoutOrder();
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
            public void DoesNotReturnMobilesWithOrderWhichAreNotNew(string orderStateString)
            {
                var orderState = new EnumConverter().ToEnum<Order.State>(orderStateString);
                var newProvision1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(orderState)
                    .Build();
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