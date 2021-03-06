﻿using FluentAssertions;
using Mobiles.Api.Domain;
using Mobiles.Api.Resources;
using System;
using Moq;
using Utils.DateTimes;
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
                var dateTimeCreatorMock = new Mock<IDateTimeCreator>();

                var expectedGuid = Guid.NewGuid();
                var expectedId = 0;
                var expectedInProgressOrderGuid = Guid.NewGuid();
                var expectedOrderToAdd = new OrderToAdd() { Name = "Neil", ContactPhoneNumber = "01234" };
                var expectedCustomerId = Guid.NewGuid();
                var phoneNumber = new PhoneNumber("07930123456");

                var sut = new MobileWhenNewBuilder(dateTimeCreatorMock.Object, expectedGuid, expectedCustomerId, phoneNumber)
                    .AddInProgressOrder(expectedOrderToAdd, expectedInProgressOrderGuid);
                var actual = sut.Build();

                actual.State.Should().Be(Mobile.MobileState.New);
                actual.Id.Should().Be(expectedId);
                actual.GlobalId.Should().Be(expectedGuid);
                actual.CustomerId.Should().Be(expectedCustomerId);
                actual.InProgressOrder.GlobalId.Should().Be(expectedInProgressOrderGuid);
                actual.InProgressOrder.Name.Should().Be(expectedOrderToAdd.Name);
                actual.InProgressOrder.ContactPhoneNumber.Should().Be(expectedOrderToAdd.ContactPhoneNumber);
            }
        }
    }
}
