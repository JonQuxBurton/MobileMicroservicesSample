using FluentAssertions;
using MobileOrderer.Api.Domain;
using MobileOrderer.Api.Resources;
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
                var expectedId = 0;
                var expectedInFlightOrderGuid = Guid.NewGuid();
                var expectedOrderToAdd = new OrderToAdd() { Name = "Neil", ContactPhoneNumber = "01234" };
                var expectedHistoryOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Buzz", ContactPhoneNumber = "05678", State = "Completed" });
                
                var sut = new MobileBuilder(expectedGuid)
                    .AddInFlightOrder(expectedOrderToAdd, expectedInFlightOrderGuid)
                    .AddOrderToHistory(expectedHistoryOrder);
                var actual = sut.Build();

                actual.CurrentState.Should().Be(Mobile.State.New);
                actual.Id.Should().Be(expectedId);
                actual.GlobalId.Should().Be(expectedGuid);
                actual.OrderHistory.First().Should().Be(expectedHistoryOrder);
                actual.InFlightOrder.GlobalId.Should().Be(expectedInFlightOrderGuid);
                actual.InFlightOrder.Name.Should().Be(expectedOrderToAdd.Name);
                actual.InFlightOrder.ContactPhoneNumber.Should().Be(expectedOrderToAdd.ContactPhoneNumber);
            }
        }
    }
}
