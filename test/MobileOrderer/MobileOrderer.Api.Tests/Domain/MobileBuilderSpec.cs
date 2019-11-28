using FluentAssertions;
using MobileOrderer.Api.Domain;
using System;
using System.Linq;
using Xunit;

namespace MobileOrderer.Api.Tests.Domain
{
    public class MobileBuilderSpec
    {
        public class BuildShould
        {
            [Fact]
            public void ReturnNewMobile()
            {
                var expectedGuid = Guid.NewGuid();
                var expectedInFlightOrder = new MobileOrder(Guid.NewGuid(), "Neil", "01234", "New");
                var expectedHistoryOrder = new MobileOrder(Guid.NewGuid(), "Buzz", "05678", "Completed");
                var sut = new MobileBuilder(Mobile.State.New, expectedGuid);

                sut.AddInFlightOrder(expectedInFlightOrder);
                sut.AddOrderToHistory(expectedHistoryOrder);
                var actual = sut.Build();

                actual.CurrentState.Should().Be(Mobile.State.New);
                actual.GlobalId.Should().Be(expectedGuid);
                actual.InFlightOrder.Should().Be(expectedInFlightOrder);
                actual.OrderHistory.First().Should().Be(expectedHistoryOrder);
            }
        }
    }
}
