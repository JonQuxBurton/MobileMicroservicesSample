﻿using FluentAssertions;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using System;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace MobileWhenNewBuilderSpec
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
                var expectedCustomerId = Guid.NewGuid();
                var phoneNumber = new PhoneNumber("07930123456");

                var sut = new MobileWhenNewBuilder(expectedGuid, expectedCustomerId, phoneNumber)
                    .AddInFlightOrder(expectedOrderToAdd, expectedInFlightOrderGuid);
                var actual = sut.Build();

                actual.CurrentState.Should().Be(Mobile.State.New);
                actual.Id.Should().Be(expectedId);
                actual.GlobalId.Should().Be(expectedGuid);
                actual.CustomerId.Should().Be(expectedCustomerId);
                actual.InFlightOrder.GlobalId.Should().Be(expectedInFlightOrderGuid);
                actual.InFlightOrder.Name.Should().Be(expectedOrderToAdd.Name);
                actual.InFlightOrder.ContactPhoneNumber.Should().Be(expectedOrderToAdd.ContactPhoneNumber);
            }
        }
    }
}