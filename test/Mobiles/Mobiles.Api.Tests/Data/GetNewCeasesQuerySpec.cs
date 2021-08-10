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
    namespace GetNewCeasesQuerySpec
    {
        public class GetShould
        {
            private readonly InMemoryDatabase<MobilesContext> database;

            public GetShould()
            {
                database = new InMemoryDatabase<MobilesContext>();
            }

            [Fact]
            public void ReturnNewActivates()
            {
                var data = new List<MobileDataEntity>
                {
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingCease.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Cease.ToString(),
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
                                State = Order.State.New.ToString()
                            }
                        }
                    },
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingCease.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Cease.ToString(),
                                State = Order.State.New.ToString()
                            }
                        }
                    },
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingCease.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Cease.ToString(),
                                State = Order.State.New.ToString()
                            }
                        }
                    }
                };

                AddToDatabase(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNewCeasesQuery(context, new DateTimeCreator());

                    var actual = sut.Get().ToList();

                    actual.Count().Should().Be(3);
                    
                    var actualMobile = actual[0];
                    actualMobile.GlobalId.Should().Be(data[0].GlobalId);
                    actualMobile.State.Should().Be(Mobile.MobileState.ProcessingCease);
                    actualMobile.InProgressOrder.Type.Should().Be(Order.OrderType.Cease);
                    actualMobile.InProgressOrder.CurrentState.Should().Be(Order.State.New);

                    actualMobile = actual[1];
                    actualMobile.GlobalId.Should().Be(data[2].GlobalId);
                    actualMobile.State.Should().Be(Mobile.MobileState.ProcessingCease);
                    actualMobile.InProgressOrder.Type.Should().Be(Order.OrderType.Cease);
                    actualMobile.InProgressOrder.CurrentState.Should().Be(Order.State.New);

                    actualMobile = actual[2];
                    actualMobile.GlobalId.Should().Be(data[3].GlobalId);
                    actualMobile.State.Should().Be(Mobile.MobileState.ProcessingCease);
                    actualMobile.InProgressOrder.Type.Should().Be(Order.OrderType.Cease);
                    actualMobile.InProgressOrder.CurrentState.Should().Be(Order.State.New);
                }
            }

            [Fact]
            public void NotReturnNonCeases()
            {
                var data = new List<MobileDataEntity>
                {
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingCease.ToString(),
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

                AddToDatabase(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNewCeasesQuery(context, new DateTimeCreator());

                    var actual = sut.Get();

                    actual.Should().BeEmpty();
                }
            }

            [Fact]
            public void NotReturnCeasesWithSentOrCompletedOrders()
            {
                var data = new List<MobileDataEntity>
                {
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingCease.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Cease.ToString(),
                                State = Order.State.Sent.ToString()
                            }
                        }
                    },
                    new MobileDataEntity
                    {
                        GlobalId = Guid.NewGuid(),
                        State = Mobile.MobileState.ProcessingCease.ToString(),
                        Orders = new List<OrderDataEntity>
                        {
                            new OrderDataEntity
                            {
                                Type = Order.OrderType.Cease.ToString(),
                                State = Order.State.Completed.ToString()
                            }
                        }
                    }
                };

                AddToDatabase(data);

                using (database)
                using (var context = new MobilesContext(database.ContextOptions))
                {
                    var sut = new GetNewCeasesQuery(context, new DateTimeCreator());

                    var actual = sut.Get();

                    actual.Should().BeEmpty();
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