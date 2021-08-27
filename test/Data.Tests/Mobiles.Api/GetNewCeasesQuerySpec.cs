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
    namespace GetNewCeasesQuerySpec
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
            public void ReturnNewCeases()
            {
                var mobileBuilder = new MobileBuilder();
                var newCease1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.ProcessingCease)
                    .WithOrderType(Order.OrderType.Cease)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newProvision1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.New)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newCease2 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.ProcessingCease)
                    .WithOrderType(Order.OrderType.Cease)
                    .WithOrderState(Order.State.New)
                    .Build();
                fixture.DataAccess.Add(newCease1);
                fixture.DataAccess.Add(newProvision1);
                fixture.DataAccess.Add(newCease2);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewCeasesQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(newCease1);
                actual.ElementAt(1).Should().BeEquivalentTo(newCease2);
                actual.Count.Should().Be(2);
            }

            [Fact]
            public void ReturnEmpty_WhenNoMobiles()
            {
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetNewCeasesQuery(context, new DateTimeCreator());

                var actual = sut.Get();

                actual.Should().BeEmpty();
            }
        }
    }
}