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
    namespace GetNewActivatesQuerySpec
    {
        public class GetShould
        {
            private readonly InMemoryMobilesDatabase database;

            public GetShould()
            {
                database = new InMemoryMobilesDatabase();
            }

            [Fact]
            public void ReturnNewActivates()
            {
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
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                State = Order.State.Processing.ToString()
                            }
                        }
                    },
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

                database.AddData(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNewActivatesQuery(context, new DateTimeCreator());

                    var actual = sut.Get().ToList();

                    actual.Count().Should().Be(3);

                    var actualMobile = actual[0];
                    actualMobile.GlobalId.Should().Be(data[0].GlobalId);
                    actualMobile.State.Should().Be(Mobile.MobileState.ProcessingActivate);
                    actualMobile.InProgressOrder.Type.Should().Be(Order.OrderType.Activate);
                    actualMobile.InProgressOrder.CurrentState.Should().Be(Order.State.New);

                    actualMobile = actual[1];
                    actualMobile.GlobalId.Should().Be(data[2].GlobalId);
                    actualMobile.State.Should().Be(Mobile.MobileState.ProcessingActivate);
                    actualMobile.InProgressOrder.Type.Should().Be(Order.OrderType.Activate);
                    actualMobile.InProgressOrder.CurrentState.Should().Be(Order.State.New);

                    actualMobile = actual[2];
                    actualMobile.GlobalId.Should().Be(data[3].GlobalId);
                    actualMobile.State.Should().Be(Mobile.MobileState.ProcessingActivate);
                    actualMobile.InProgressOrder.Type.Should().Be(Order.OrderType.Activate);
                    actualMobile.InProgressOrder.CurrentState.Should().Be(Order.State.New);

                }
            }

            [Fact]
            public void NotReturnNonActivates()
            {
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
                                Type = Order.OrderType.Provision.ToString(),
                                State = Order.State.New.ToString()
                            }
                        }
                    }
                };

                database.AddData(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNewActivatesQuery(context, new DateTimeCreator());

                    var actual = sut.Get();

                    actual.Should().BeEmpty();
                }
            }

            [Fact]
            public void NotReturnProvisionsWithSentOrCompletedOrders()
            {
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
                                State = Order.State.Sent.ToString()
                            }
                        }
                    },
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Activate.ToString(),
                                State = Order.State.Completed.ToString()
                            }
                        }
                    }
                };

                database.AddData(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNewActivatesQuery(context, new DateTimeCreator());

                    var actual = sut.Get();

                    actual.Should().BeEmpty();
                }
            }
        }
    }
}