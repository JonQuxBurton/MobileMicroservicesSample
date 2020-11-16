using FluentAssertions;
using Mobiles.Api.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Utils.DateTimes;
using Xunit;

namespace Mobiles.Api.Tests.Domain
{
    namespace MobileSpec
    {
        public class ConstructorShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public ConstructorShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void CreateMobileInStateNew()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity{ Id = 101, GlobalId = Guid.NewGuid(), State = "New" });

                sut.State.Should().Be(Mobile.MobileState.New);
            }

            [Fact]
            public void CreateMobile()
            {
                var expectedGlobalId = Guid.NewGuid();
                var expectedId = 101;
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity {Id = expectedId, GlobalId = expectedGlobalId, State = "New" });

                sut.GlobalId.Should().Be(expectedGlobalId);
                sut.Id.Should().Be(expectedId);
            }
        }

        public class ProvisionShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public ProvisionShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void ChangeCurrentStateToPendingActivation()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    Id = 101, GlobalId = Guid.NewGuid() , State = "New",
                    Orders = new List<OrderDataEntity>()
                    {
                        mobileOrder.GetDataEntity()
                    }
                });

                sut.Provision();

                sut.State.Should().Be(Mobile.MobileState.ProcessingProvision);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid() , State = "New", Orders = new List<OrderDataEntity>(){ mobileOrder.GetDataEntity() }

                    });

                sut.Provision();

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }

        public class ActivateShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public ActivateShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void ChangeCurrentStateToProcessingActivate()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.WaitingForActivate.ToString() });

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.State.Should().Be(Mobile.MobileState.ProcessingActivate);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.WaitingForActivate.ToString() 

                    });

                sut.Activate(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }
        
        public class ActivateCompletedShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public ActivateCompletedShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void ChangeCurrentStateToLive()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingActivate.ToString() });

                sut.ActivateCompleted();

                sut.State.Should().Be(Mobile.MobileState.Live);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    });

                sut.ActivateCompleted();

                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInFlightOrderToNull()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingActivate.ToString(),
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    } 
                    );

                sut.ActivateCompleted();

                sut.InFlightOrder.Should().BeNull();
            }
        }

        public class OrderProcessingShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public OrderProcessingShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Theory]
            [InlineData("PendingActivation")]
            [InlineData("PendingLive")]
            public void SetInFlightOrderStateToProcessing(string mobileState)
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = mobileState ,
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    });

                sut.OrderProcessing();

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.Processing);
            }
        }

        public class OrderSentShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public OrderSentShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Theory]
            [InlineData("PendingActivation")]
            [InlineData("PendingLive")]
            public void SetInFlightOrderStateToSent(string mobileState)
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Processing" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = mobileState,
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    }
                    );

                sut.OrderSent();

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.Sent);
            }
        }

        public class CeaseShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public CeaseShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void ChangeCurrentStateToProcessingCease()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "Live" });

                sut.Cease(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.State.Should().Be(Mobile.MobileState.ProcessingCease);
            }

            [Fact]
            public void SetInFlightOrderStateToNew()
            {
                //var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = "Live" });

                sut.Cease(new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "New" }));

                sut.InFlightOrder.CurrentState.Should().Be(Order.State.New);
            }
        }

        public class CeaseCompletedShould
        {
            private readonly Mock<IDateTimeCreator> dateTimeCreatorMock;

            public CeaseCompletedShould()
            {
                dateTimeCreatorMock = new Mock<IDateTimeCreator>();
            }

            [Fact]
            public void ChangeCurrentStateToCeased()
            {
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity { Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingCease.ToString() });

                sut.CeaseCompleted();

                sut.State.Should().Be(Mobile.MobileState.Ceased);
            }

            [Fact]
            public void SetActivateOrderStateToCompleted()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(dateTimeCreatorMock.Object, new MobileDataEntity
                {
                    Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingCease.ToString(),
                    Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                });

                sut.CeaseCompleted();

                sut.Orders.First().CurrentState.Should().Be(Order.State.Completed);
            }

            [Fact]
            public void SetInFlightOrderToNull()
            {
                var mobileOrder = new Order(new OrderDataEntity { GlobalId = Guid.NewGuid(), Name = "Name", ContactPhoneNumber = "0123456789", State = "Sent" });
                var sut = new Mobile(dateTimeCreatorMock.Object,
                    new MobileDataEntity
                    {
                        Id = 101, GlobalId = Guid.NewGuid(), State = Mobile.MobileState.ProcessingCease.ToString(),
                        Orders = new List<OrderDataEntity>() { mobileOrder.GetDataEntity() }
                    }
                );

                sut.CeaseCompleted();

                sut.InFlightOrder.Should().BeNull();
            }
        }
    }
}
