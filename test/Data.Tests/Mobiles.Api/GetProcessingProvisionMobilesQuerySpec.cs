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
    namespace GetProcessingProvisionMobilesQuerySpec
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
            public void ReturnNewProcessingProvisions()
            {
                var mobileBuilder = new MobileBuilder();
                var newProcessingProvisions1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.ProcessingProvision)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newCease1 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.ProcessingCease)
                    .WithOrderType(Order.OrderType.Cease)
                    .WithOrderState(Order.State.New)
                    .Build();
                var newProcessingProvisions2 = mobileBuilder
                    .WithMobileState(Mobile.MobileState.ProcessingProvision)
                    .WithOrderType(Order.OrderType.Provision)
                    .WithOrderState(Order.State.New)
                    .Build();
                fixture.DataAccess.Add(newProcessingProvisions1);
                fixture.DataAccess.Add(newCease1);
                fixture.DataAccess.Add(newProcessingProvisions2);
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetProcessingProvisionMobilesQuery(context, new DateTimeCreator());

                var actual = sut.Get().ToList();

                actual.ElementAt(0).Should().BeEquivalentTo(newProcessingProvisions1);
                actual.ElementAt(1).Should().BeEquivalentTo(newProcessingProvisions2);
                actual.Count.Should().Be(2);
            }

            [Fact]
            public void ReturnEmpty_WhenNoMobiles()
            {
                using var context = new MobilesContext(fixture.ContextOptions);
                var sut = new GetProcessingProvisionMobilesQuery(context, new DateTimeCreator());

                var actual = sut.Get();

                actual.Should().BeEmpty();
            }
        }
    }
}