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
    namespace GetMobilesByCustomerIdQuerySpec
    {
        public class GetShould
        {
            private readonly InMemoryMobilesDatabase database;

            public GetShould()
            {
                database = new InMemoryMobilesDatabase();
            }

            [Fact]
            public void ReturnMobileWithMatchingCustomerId()
            {
                var expectedCustomerId = Guid.NewGuid();
                
                var data = new List<MobileDataEntity>
                {
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        CustomerId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString()
                    },
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        CustomerId = expectedCustomerId,
                        State = Mobile.MobileState.ProcessingActivate.ToString()
                    },
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        CustomerId = expectedCustomerId,
                        State = Mobile.MobileState.Live.ToString()
                    }
                };

                database.AddData(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetMobilesByCustomerIdQuery(context, new DateTimeCreator());

                    var actual = sut.Get(expectedCustomerId).ToList();

                    actual.Count().Should().Be(2);

                    var actualMobile = actual[0];
                    actualMobile.GlobalId.Should().Be(data[1].GlobalId);
                    actualMobile.CustomerId.Should().Be(data[1].CustomerId);
                    actualMobile.State.ToString().Should().Be(data[1].State);
                    
                    actualMobile = actual[1];
                    actualMobile.GlobalId.Should().Be(data[2].GlobalId);
                    actualMobile.CustomerId.Should().Be(data[2].CustomerId);
                    actualMobile.State.ToString().Should().Be(data[2].State);
                }
            }

            [Fact]
            public void ReturnEmptyWhenNoneMatchCustomerId()
            {
                var notFoundCustomerId = Guid.NewGuid();
                var data = new List<MobileDataEntity>
                {
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        CustomerId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                    }
                };

                database.AddData(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetMobilesByCustomerIdQuery(context, new DateTimeCreator());

                    var actual = sut.Get(notFoundCustomerId).ToList();

                    actual.Should().BeEmpty();
                }
            }
        }
    }
}