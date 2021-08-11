using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Mobiles.Api.Data;
using Mobiles.Api.Domain;
using Utils.DateTimes;
using Xunit;

namespace Mobiles.Api.Tests.Data
{
    namespace GetNextMobileIdQuerySpec
    {
        public class GetShould
        {
            private readonly InMemoryMobilesDatabase database;

            public GetShould()
            {
                database = new InMemoryMobilesDatabase();
            }

            [Fact]
            public void ReturnOneGreaterThanExistingMaxId()
            {
                var expectedOrder = new OrderDataEntity
                {
                    Id = 1,
                    GlobalId = Guid.NewGuid(),
                    Type = Order.OrderType.Activate.ToString(),
                    State = Order.State.New.ToString()
                };
                var expectedMobile = new MobileDataEntity
                {
                    GlobalId = Guid.NewGuid(),
                    State = Mobile.MobileState.ProcessingActivate.ToString(),
                    Orders = new List<OrderDataEntity>
                    {
                        expectedOrder
                    }
                };

                var data = new List<MobileDataEntity>
                {
                    expectedMobile,

                };

                database.AddData(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNextMobileIdQuery(context);

                    var actual = sut.Get();

                    actual.Should().Be(2);
                }
            }

            [Fact]
            public void ReturnOneWhenNoMobilesExist()
            {
                database.SetupAsEmpty();

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNextMobileIdQuery(context);

                    var actual = sut.Get();

                    actual.Should().Be(1);
                }
            }
        }
    }
}