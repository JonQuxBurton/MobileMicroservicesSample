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
    namespace GetMobileByOrderIdQuerySpec
    {
        public class GetShould
        {
            private readonly InMemoryDatabase<MobilesContext> database;

            public GetShould()
            {
                database = new InMemoryDatabase<MobilesContext>();
            }

            [Fact]
            public void ReturnMobileWithOrderMatchingSuppliedOrderId()
            {
                var expectedOrder = new OrderDataEntity
                {
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
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Activate.ToString(),
                                State = Order.State.New.ToString()
                            }
                        }
                    },
                    expectedMobile,
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Activate.ToString(),
                                State = Order.State.New.ToString()
                            }
                        }
                    }
                };

                AddToDatabase(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetMobileByOrderIdQuery(context, new DateTimeCreator());

                    var actual = sut.Get(expectedOrder.GlobalId);

                    actual.GlobalId.Should().Be(expectedMobile.GlobalId);
                    actual.State.ToString().Should().Be(expectedMobile.State);
                    var actualOrder = actual.Orders.First();
                    actualOrder.GlobalId.Should().Be(expectedOrder.GlobalId);
                    actualOrder.Type.ToString().Should().Be(expectedOrder.Type);
                    actualOrder.CurrentState.ToString().Should().Be(expectedOrder.State);
                }
            }            
            
            [Fact]
            public void ReturnNullWhenOrderIdNotFound()
            {
                var notFoundOrderId = Guid.NewGuid();
                var data = new List<MobileDataEntity>
                {
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                GlobalId = Guid.NewGuid(),
                                Type = Order.OrderType.Activate.ToString(),
                                State = Order.State.New.ToString()
                            }
                        }
                    }
                };

                AddToDatabase(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetMobileByOrderIdQuery(context, new DateTimeCreator());

                    var actual = sut.Get(notFoundOrderId);

                    actual.Should().BeNull();
                }
            }

            private void AddToDatabase(List<MobileDataEntity> data)
            {
                using var context = new MobilesContext(database.ContextOptions);
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                foreach (var dataEntity in data) context.Mobiles.Add(dataEntity);

                context.SaveChanges();
            }
        }
    }
}